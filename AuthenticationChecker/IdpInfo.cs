using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthenticationChecker
{
  public class IdpInfo
  {
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string DiscoveryEndpoint { get; set; }
    public string Issuer { get; set; }
    public string Scope { get; set; }
  }
}