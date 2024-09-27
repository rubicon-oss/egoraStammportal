using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthenticationChecker
{
  public class UserMapping
  {
    public List<Mapping> Mappings;
  }

  public class Mapping
  {
    public IdAustriaIdentity IdAustria;
    public AdIdentity Ad;
  }

  public class IdAustriaIdentity
  {
    public string GivenName;
    public string FamilyName;
    public DateTime DateOfBirth;
  }

  public class AdIdentity
  {
    public string GivenName;
    public string Sn;
  }
}