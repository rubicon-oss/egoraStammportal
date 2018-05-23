using System;
using System.Collections.Generic;
using System.Text;
using Egora.Stammportal.LdapAuthorizationService;

namespace LdapAuthorizationServiceTest
{
  public class ValueFormatter :IValueFormatter
  {
    public string Format(string value)
    {
      return value + " through formatter";
    }
  }
}
