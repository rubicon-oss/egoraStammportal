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

namespace Egora.Stammportal.LdapAuthorizationService
{
  public class PvpRole
  {
    public static PvpRole Parse(string roleString)
    {
      PvpRole role = new PvpRole();
      string[] parts = roleString.Split('(', ')');
      if (parts.Length > 0)
      {
        role._name = parts[0];
      }
      if (parts.Length > 2)
      {
        string[] parameters = parts[1].Split(',');
        foreach (string parameter in parameters)
        {
          string[] keyvalue = parameter.Split('=');
          string key = string.Empty;
          string val = string.Empty;
          if (keyvalue.Length > 0)
            key = keyvalue[0];
          if (keyvalue.Length > 1)
            val = keyvalue[1];
          role._parameters.Add(key, val);
        }
      }
      return role;
    }

    private NameValueCollection _parameters = new NameValueCollection();

    public NameValueCollection Parameters
    {
      get { return _parameters; }
      set { _parameters = value; }
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
          foreach (string value in Parameters.GetValues(key))
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
      foreach (string key in parameters.AllKeys)
      {
        List<string> currentValues = new List<string>(_parameters.GetValues(key));
        foreach (string val in parameters.GetValues(key))
        {
          if (!currentValues.Contains(val))
            _parameters.Add(key, val);
        }
      }
    }

    public string GetSoapFragment()
    {
      if (String.IsNullOrEmpty(_name))
        return null;

      StringBuilder sb =
        new StringBuilder("<role value=\"" + PvpApplicationLdapAuthorizer.XmlEncode(_name) + "\">\n");
      if (Parameters.HasKeys())
      {
        sb.Append("<param>");
        foreach (string key in Parameters.AllKeys)
        {
          foreach (string value in Parameters.GetValues(key))
          {
            sb.Append("<key>" + PvpApplicationLdapAuthorizer.XmlEncode(key)
                      + "</key><value>" + PvpApplicationLdapAuthorizer.XmlEncode(value) + "</value>\n");
          }
        }
        sb.Append("</param>");
      }
      sb.Append("</role>");
      return sb.ToString();
    }
  }
}