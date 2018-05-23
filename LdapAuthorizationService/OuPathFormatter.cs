using System;
using System.Collections.Generic;
using System.Text;

namespace Egora.Stammportal.LdapAuthorizationService
{
  public class OuPathFormatter : IValueFormatter
  {
    public string Format(string value)
    {
      string[] parts = value.Split(',');
      
      List<string> ous = new List<string>();
      foreach (string part in parts)
      {
        if (part.StartsWith("OU=", StringComparison.InvariantCultureIgnoreCase))
          ous.Add(part.Substring(3).Replace(' ', '_'));
      }

      return String.Join("/", ous.ToArray());
    }
  }
}
