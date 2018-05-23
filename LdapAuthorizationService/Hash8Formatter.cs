using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Egora.Stammportal.LdapAuthorizationService
{
  public class Hash8Formatter : IValueFormatter
  {
    public string Format(string value)
    {
      using (var sha1 = new SHA1Managed())
      {
        if (value == null)
          return String.Empty;

        var hash = Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(value)));
        return hash.Substring(0, 8);
      }
    }
  }
}
