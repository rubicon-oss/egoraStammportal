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
      var samlAttributes = GetSamlAttributes(partner, userName);

      SAMLIdentityProvider.SendSSO(context.Response, userName, samlAttributes);
    }

    public static SAMLAttribute[] GetSamlAttributes(PartnerServiceProviderConfiguration partner, string userName)
    {
      string rootUrl = partner.AssertionConsumerServiceUrl;
      var authorizer = new PvpAuthorizerSoapClient();
      Egora.Stammportal.PvpIdentityProvider.AuthorizationWebService.CustomAuthorization authorization = authorizer.GetAuthorization(rootUrl, userName);
      if (authorization == null || authorization.Equals(Egora.Stammportal.CustomAuthorization.NoAuthorization))
      {
        throw new ApplicationException("No Authorization received.");

      }

      if (!authorization.PvpVersion.Contains("2"))
        throw new ApplicationException($"Pvp Version 2.0 or 2.1 expected, but {authorization.PvpVersion} received");

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