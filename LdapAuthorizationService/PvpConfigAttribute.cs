/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.Xml.Serialization;
using Egora.Pvp;

namespace Egora.Stammportal.LdapAuthorizationService
{
  public enum PropertySource
  {
    Undefined = 0,
    User = 1,
    Group = 2,
    UserOrGroup = 3,
    OuPath=4,
  }

  public class PvpConfigAttribute
  {
    private string _name;
    private PvpAttributes? _key;

    [XmlAttribute("name")]
    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;

        if (!_key.HasValue && PvpLegacy.HeaderMapping.ContainsKey(_name))
          _key = PvpLegacy.HeaderMapping[_name];
      }
    }

    [XmlAttribute("key")]
    public PvpAttributes Key
    {
      get
      {
        if (_key.HasValue)
          return _key.Value;
        throw new PvpException("Key nicht gesetzt.");
      }
      set { _key = value; }
    }

    private string _ldapAttribute;

    [XmlAttribute("ldapAttribute")]
    public string LdapAttribute
    {
      get
      {
        if (_ldapAttribute != null)
          return _ldapAttribute;
        if (GlobalConfigAttribute != null)
          return GlobalConfigAttribute.LdapAttribute;
        return null;
      }
      set { _ldapAttribute = value; }
    }

    private string _format;

    [XmlAttribute("format")]
    public string Format
    {
      get
      {
        if (_format != null)
          return _format;
        if (GlobalConfigAttribute != null)
          return GlobalConfigAttribute.Format;
        return "{0}";
      }

      set { _format = value; }
    }

    private string _formatter;
    [XmlAttribute("formatter")]
    public string Formatter
    {
      get
      {
        if (_formatter != null)
          return _formatter;
        if (GlobalConfigAttribute != null)
          return GlobalConfigAttribute.Formatter;
        return null;
      }

      set { _formatter = value; }
    }

    private string _defaultValue;

    [XmlAttribute("default")]
    public string DefaultValue
    {
      get
      {
        if (_defaultValue != null)
          return _defaultValue;
        if (GlobalConfigAttribute != null)
          return GlobalConfigAttribute.DefaultValue;
        return null;
      }

      set { _defaultValue = value; }
    }

    private PropertySource _source = PropertySource.Undefined;

    [XmlAttribute("source")]
    public PropertySource Source
    {
      get
      {
        if (_source != PropertySource.Undefined)
          return _source;
        if (GlobalConfigAttribute != null)
          return GlobalConfigAttribute.Source;
        return PropertySource.User;
      }
      set { _source = value; }
    }

    private PvpConfigAttribute _globalConfigAttribute;

    [XmlIgnore]
    internal PvpConfigAttribute GlobalConfigAttribute
    {
      get { return _globalConfigAttribute; }
      set { _globalConfigAttribute = value; }
    }

    public string[] GetValue(DirectoryEntry user, List<DirectoryEntry> groups)
    {
      if (user == null)
        return null;

      if (Source == PropertySource.User)
      {
        string propValue = GetEntryValue(user);
        if (propValue != null)
          return new string[] {FormatValue(propValue)};
        return new string[] {DefaultValue};
      }

      if (Source == PropertySource.UserOrGroup)
      {
        string propValue = GetEntryValue(user);
        if (propValue != null)
          return new string[] {FormatValue(propValue)};

        return GetGroupValue(groups);
      }

      if (Source == PropertySource.Group)
        return GetGroupValue(groups);

      if (Source == PropertySource.OuPath)
      {
        string propValue=GetOuPathValue(user.Parent);
        if (propValue != null)
          return new string[] { FormatValue(propValue) };
        return new string[] { DefaultValue };
      }
      return null;
    }

    private string FormatValue(string propValue)
    {
      if (String.IsNullOrEmpty(Formatter))
        return propValue==null ? null : String.Format(Format, propValue);

      Type formatterType = Type.GetType(Formatter);
      IValueFormatter formatter = Activator.CreateInstance(formatterType) as IValueFormatter;
      if (formatter == null)
        return String.Format(Format, propValue);
      
      return formatter.Format(propValue);
    }

    private string GetEntryValue(DirectoryEntry entry)
    {
      if (String.IsNullOrEmpty(LdapAttribute))
        return null;

      string propValue = null;
      if (entry.Properties.Contains(LdapAttribute))
        propValue = entry.Properties[LdapAttribute].Value.ToString();

      return propValue;
    }

    private string[] GetGroupValue(List<DirectoryEntry> groups)
    {
      List<string> values = new List<string>();
      foreach (DirectoryEntry group in groups)
      {
        string s = FormatValue(GetEntryValue(group));
        if (s!=null)
         values.Add(s);
      }
      if (values.Count == 0)
        values.Add(DefaultValue);

      return values.ToArray();
    }

    private string GetOuPathValue(DirectoryEntry ou)
    {
      string path = ou.Path;
      if (!(ou.Properties["objectClass"].Contains("organizationalUnit")))
        return null;
      
      string propValue = GetEntryValue(ou);
      
      if (string.IsNullOrEmpty(propValue))
        return GetOuPathValue(ou.Parent);
      
      return propValue;
    }

  }
}