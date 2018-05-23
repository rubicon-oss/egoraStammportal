/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Egora.Pvp
{
  public class PvpRole
  {
    private const string c_lpSubstitute = "{E566EB71-127F-489b-AC57-66C5E61B5164}";
    private const string c_rpSubstitute = "{C7BCCAF9-9F7C-4af7-9AC7-A063A98E0B68}";
    private const string c_cSubstitute = "{7373FB41-BD65-4b42-BA09-74541D901A85}";
    private const string c_bsSubstitute = "{AAD80981-ABD4-4b12-831B-12B64BB0F8D4}";

    public static PvpRole Parse(string roleString)
    {
      return Parse(roleString, false);
    }

    public static PvpRole Parse(string roleString, bool decode)
    {
      PvpRole role = new PvpRole();
      if (decode)
        roleString = roleString.Replace(@"\(", c_lpSubstitute).Replace(@"\)", c_rpSubstitute);
      string[] parts = roleString.Split('(', ')');
      if (parts.Length > 0)
      {
        role._name = decode ? Decode(parts[0].Replace(c_lpSubstitute, "(").Replace(c_rpSubstitute, ")")) : parts[0];
      }
      if (parts.Length >= 2)
      {
        string[] parameters = (decode ? parts[1].Replace(@"\,", c_cSubstitute) : parts[1]).Split(',');
        foreach (string parameter in parameters)
        {
          if (String.IsNullOrEmpty(parameter))
            continue;
          string p = decode ? Decode(parameter.Replace(c_cSubstitute, ",")) : parameter;
          string[] keyvalue = p.Split('=');
          string key = string.Empty;
          string val = null;
          if (keyvalue.Length > 0)
            key = keyvalue[0];
          if (keyvalue.Length > 1)
            val = keyvalue[1];
          role._parameters.Add(key, val);
        }
      }
      return role;
    }

    public static string Decode(string roleString)
    {
      StringBuilder sb = new StringBuilder(roleString);
      sb.Replace(@"\\", c_bsSubstitute);
      sb.Replace(@"\s", " ");
      sb.Replace(@"\r", "\r");
      sb.Replace(@"\n", "\n");
      sb.Replace(@"\,", ",");
      sb.Replace(@"\;", ";");
      sb.Replace(@"\)", ")");
      sb.Replace(@"\(", "(");
      sb.Replace(c_bsSubstitute, @"\");
      return sb.ToString();
    }

    public PvpRole()
    { }

    public PvpRole(string roleName)
    {
      Name = roleName;
    }

    public PvpRole(string roleName, NameValueCollection parameters)
    {
      Name = roleName;
      Merge(parameters);
    }

    private NameValueCollection _parameters = new NameValueCollection();

    public NameValueCollection Parameters
    {
      get { return _parameters; }
      set { _parameters = value; }
    }

    public string[] GetValuesForParameter(string parameterName)
    {
      return Parameters.GetValues(parameterName);
    }

    private string _name;

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder(_name);
      if (Parameters.HasKeys())
      {
        sb.Append("(");
        bool first = true;
        foreach (string key in Parameters.AllKeys)
        {
          string[] values = Parameters.GetValues(key);
          if (values == null)
            values = new string[] {String.Empty};
          foreach (string value in values)
          {
            if (!first)
              sb.Append(",");
            first = false;

            if (String.IsNullOrEmpty(value))
            {
              sb.Append(key);
            }
            else
            {
              sb.Append(key + "=" + value);
            }
          }
        }
        sb.Append(")");
      }

      return sb.ToString();
    }

    public void Merge(NameValueCollection parameters)
    {
      if (parameters == null)
        return;

      foreach (string key in parameters.AllKeys)
      {
        List<string> currentValues = new List<string>(_parameters.GetValues(key) ?? new string[] {});
        foreach (string val in parameters.GetValues(key) ?? new string[] {})
        {
          string cv = val;
          if (!currentValues.Exists(v => v.Equals(cv, StringComparison.InvariantCultureIgnoreCase)))
            _parameters.Add(key, val);
        }
      }
    }

    public string GetSoapFragment()
    {
      if (String.IsNullOrEmpty(_name))
        return null;

      StringBuilder sb =
        new StringBuilder("<role value=\"" + EncodingUtil.XmlEncode(_name) + "\">\n");
      if (Parameters.HasKeys())
      {
        foreach (string key in Parameters.AllKeys)
        {
          sb.Append("<param>\n");
          sb.Append("<key>" + EncodingUtil.XmlEncode(key) + "</key>");
          foreach (string value in Parameters.GetValues(key) ?? new string[] { })
          {
            sb.Append("<value>" + EncodingUtil.XmlEncode(value) + "</value>");
          }
        sb.Append("\n</param>\n");
        }
      }
      sb.Append("</role>");
      return sb.ToString();
    }

    public bool Contains(PvpRole pvpRole)
    {
      foreach (string key in pvpRole.Parameters.AllKeys)
      {
        string[] values = Parameters.GetValues(key);
        string[] compareValues = pvpRole.Parameters.GetValues(key);

        if (compareValues == null)
          continue;

        if (values == null)
          return false;

        List<string> vals = new List<string>(values);
        foreach (string compareVal in compareValues)
        {
          string cv = compareVal;
          if (!vals.Exists(v => v.Equals(cv, StringComparison.InvariantCultureIgnoreCase)))
            return false;
        }
      }

      return true;
    }
  }
}