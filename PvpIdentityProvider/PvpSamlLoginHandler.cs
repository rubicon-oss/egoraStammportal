using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using System.Xml.Linq;
using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Assertions;
using ComponentSpace.SAML2.Configuration;
using Egora.Pvp;
using Egora.Stammportal.PvpIdentityProvider.AuthorizationWebService;
using static System.Net.WebRequestMethods;

namespace Egora.Stammportal.PvpIdentityProvider
{
  public class PvpSamlLoginHandler : IHttpHandler, IRequiresSessionState
  {
    public void ProcessRequest(HttpContext context)
    {
      string userName = null;
      userName = GetUserName(context.Request);
      string serviceProvider;
      SAMLIdentityProvider.ReceiveSSO(context.Request, out serviceProvider);
      var partner = SAMLController.Configuration.GetPartnerServiceProvider(serviceProvider);
      var samlAttributes = GetSamlAttributes(partner, userName, out var pvpVersion, out var secClass, out var authnContext);

      if (secClass >= 3)
      {
        // ToDo Check 2nd factor
        throw new ApplicationException($"SecClass {secClass} not yet supported.");
      }

      string assertionConsumerServiceUrl = partner.AssertionConsumerServiceUrl;
      SAMLIdentityProvider.SendSSO(context.Response, userName, samlAttributes, authnContext, assertionConsumerServiceUrl);
    }

    public static SAMLAttribute[] GetSamlAttributes(PartnerServiceProviderConfiguration partner, string userName,
      out string pvpVersion, out int secClass, out string authnContext)
    {
      string rootUrl = partner.AssertionConsumerServiceUrl;
      var authorizer = new PvpAuthorizerSoapClient();
      Egora.Stammportal.PvpIdentityProvider.AuthorizationWebService.CustomAuthorization authorization = authorizer.GetAuthorization(rootUrl, userName);
      if (authorization == null || authorization.HttpHeaders == null)
        throw new ApplicationException("No Authorization received.");

      var noAuth = authorization.HttpHeaders[0];
      var noAuthorizationHttpHeader = Egora.Stammportal.CustomAuthorization.NoAuthorization.HttpHeaders[0];
      if (noAuth.Name == noAuthorizationHttpHeader.Name && noAuth.Value == noAuthorizationHttpHeader.Value)
        throw new ApplicationException("NoAuthorization received.");

      if (!authorization.PvpVersion.Contains("2"))
        throw new ApplicationException($"Pvp Version 2.0 or 2.1 expected, but {authorization.PvpVersion} received");

      pvpVersion = authorization.PvpVersion;

      secClass = 0;
      var pvpSecClass = authorization.HttpHeaders.Where(h => h.Name.Equals("X-PVP-SECCLASS", StringComparison.InvariantCultureIgnoreCase))
        .Select(h => h.Value).Max();
      if (pvpSecClass != null)
        int.TryParse(pvpSecClass, out secClass);

      authnContext = pvpVersion.Contains("2.1")
        ? secClass == 0 ? "http://www.ref.gv.at/ns/names/agiz/pvp/secclass/0" : $"http://www.ref.gv.at/ns/names/agiz/pvp/secclass/0-{secClass}"
        : $"http://www.ref.gv.at/ns/names/agiz/pvp/secclass/{secClass}";

      var attributes = authorization.SoapHeaderXmlFragment;
      var samlAttributes = attributes.Elements().Select(CreateSamlAttribute).ToArray();
      return samlAttributes;
    }

    public static string GetUserName(HttpRequest request)
    {
      string userName = null;
      if (request.LogonUserIdentity != null)
      {
        userName = request.LogonUserIdentity.Name;
      }
      else if (WindowsIdentity.GetCurrent() != null)
      {
        userName = WindowsIdentity.GetCurrent().Name;
      }
      return userName;
    }

    private static SAMLAttribute CreateSamlAttribute(XElement a)
    {
      SAMLAttribute attribute= new SAMLAttribute(a.Attribute("Name").Value, a.Attribute("NameFormat").Value, a.Attribute("FriendlyName").Value);
      attribute.Values = a.Elements().Select(v => new AttributeValue(v.Value)).ToList();
      return attribute;
    }

    public bool IsReusable
    {
      get { return false; }
    }
  }
}