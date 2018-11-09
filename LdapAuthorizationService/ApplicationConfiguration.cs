/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Egora.Pvp;

namespace Egora.Stammportal.LdapAuthorizationService
{
  [Serializable()]
  public class ApplicationConfiguration
  {
    private PvpConfigAttribute[] _pvpConfigAttributesXml;

    [XmlElement("PvpAttribute")]
    public PvpConfigAttribute[] PvpConfigAttributes
    {
      get { return _pvpConfigAttributesXml; }
      set { _pvpConfigAttributesXml = value; }
    }

    private XmlAttribute[] _allAttributes;

    [XmlAnyAttribute]
    public XmlAttribute[] AllAttributes
    {
      get { return _allAttributes; }
      set { _allAttributes = value; }
    }
    
    [XmlIgnore]
    public PvpAttributes[] AllDefinedPvpAttributes
    {
      get
      {
        List<PvpAttributes> attrs = PvpConfigAttributes == null ? new List<PvpAttributes>() : new List<PvpAttributes>(PvpConfigAttributes.Select(a => a.Key));
        attrs.AddRange(
          LdapConfiguration.GetConfiguration().GlobalApplication.PvpConfigAttributes.Where(ga => !attrs.Contains(ga.Key)).Select(a => a.Key));
        return attrs.ToArray();
      }
    }

    private Dictionary<PvpAttributes, PvpConfigAttribute> _pvpAttributes;

    public PvpConfigAttribute GetPvpAttribute(PvpAttributes key)
    {
      if (_pvpAttributes == null)
      {
        _pvpAttributes=CreatePvpAttributesDictionary();
      }

      PvpConfigAttribute attr;
      if (_pvpAttributes.TryGetValue(key, out attr))
        return attr;
      
      ApplicationConfiguration globalApp = LdapConfiguration.GetConfiguration().GlobalApplication;
      if (this != globalApp)
        return globalApp.GetPvpAttribute(key);
      
      return null;
    }

    private Dictionary<PvpAttributes, PvpConfigAttribute> CreatePvpAttributesDictionary()
    {
      Dictionary<PvpAttributes, PvpConfigAttribute>  pvpAttributes = new Dictionary<PvpAttributes, PvpConfigAttribute>();
      if (_pvpConfigAttributesXml != null)
      {
        foreach (PvpConfigAttribute attrXml in _pvpConfigAttributesXml)
        {
          ApplicationConfiguration global = LdapConfiguration.GetConfiguration().GlobalApplication;
          if (this == global || attrXml.BlockGlobal)
          {
            attrXml.GlobalConfigAttribute = null;
          }
          else
          {
            attrXml.GlobalConfigAttribute = global.GetPvpAttribute(attrXml.Key);
          }
          pvpAttributes.Add(attrXml.Key, attrXml);
        }
      }
      return pvpAttributes;
    }

    public string GetConfigValue(string localName)
    {
      foreach (XmlAttribute attr in AllAttributes)
      {
        if (attr.LocalName == localName && !String.IsNullOrEmpty(attr.Value))
          return attr.Value;
      }
      return null;
    }

    public string GetConfigValueWithGlobal(string localName)
    {
      string localValue = GetConfigValue(localName);
      if (localValue == null)
      {
        ApplicationConfiguration global = LdapConfiguration.GetConfiguration().GlobalApplication;
        if (this != global)
          return global.GetConfigValue(localName);
      }
      return localValue;
    }

    public string[] GetAttributeValue(PvpAttributes pvpAttribute, DirectoryEntry user, List<DirectoryEntry> groups)
    {
      PvpConfigAttribute attr = GetPvpAttribute(pvpAttribute);
      if (attr != null)
        return attr.GetValue(user, groups);
      return null;
    }

    [XmlIgnore]
    public string Name
    {
      get { return GetConfigValue("name"); }
    }

    [XmlIgnore]
    public string Key
    {
      get { return GetConfigValue("key"); }
    }

    [XmlIgnore]
    public string DomainPrefix
    {
      get { return GetConfigValueWithGlobal("domainPrefix"); }
    }

    [XmlIgnore]
    public string LdapRoot
    {
      get { return GetConfigValueWithGlobal("ldapRoot"); }
    }

    [XmlIgnore]
    public string UserProperties
    {
      get { return GetConfigValue("userProperties"); }
    }

    [XmlIgnore]
    public int AuthorizationTimeToLive
    {
      get { return Convert.ToInt32(GetConfigValueWithGlobal("authorizationTimeToLive")); }
    }

    [XmlIgnore]
    public string SoapPrincipalFragment
    {
      get { return GetConfigValueWithGlobal("soapPrincipal"); }
    }

    [XmlIgnore]
    public string WebUrls
    {
      get { return GetConfigValue("webUrls"); }
    }

    [XmlIgnore]
    public string SoapUrls
    {
      get { return GetConfigValue("soapUrls"); }
    }

    [XmlIgnore]
    public string SamlUrls
    {
      get { return GetConfigValue("samlUrls"); }
    }

    [XmlIgnore]
    public string GroupContainer
    {
      get { return GetConfigValue("groupContainer"); }
    }

    [XmlIgnore]
    public bool RecurseGroupMembership
    {
      get { return GetConfigValue("recurseGroupMembership") == "true"; }
    }

    [XmlIgnore]
    public bool MustHaveRole
    {
      get { return GetConfigValue("mustHaveRole") == "true"; }
    }

    public bool IsWeb(string rootUrl)
    {
      return (WebUrls != null && WebUrls.Contains(rootUrl));
    }

    public bool IsSoap(string rootUrl)
    {
      return (SoapUrls != null && SoapUrls.Contains(rootUrl));
    }

    public bool IsSaml(string rootUrl)
    {
      return (SamlUrls != null && SamlUrls.Contains(rootUrl));
    }
  }
}