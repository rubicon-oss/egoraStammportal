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
using Egora.Stammportal.PvpIdentityProvider.AuthorizationWebService;

namespace Egora.Stammportal.PvpIdentityProvider
{
  public class SamlLogoutHandler : IHttpHandler, IRequiresSessionState
  {
    public void ProcessRequest(HttpContext context)
    {
      string serviceProvider;
      bool isRequest;
      bool hasCompleted;
      string reason;
      string relayState;
      SAMLIdentityProvider.ReceiveSLO(context.Request, context.Response, out isRequest, out hasCompleted, out reason, out serviceProvider, out relayState);
      
      //only on ServiceProvider allowed in this sample
      if (isRequest)
        SAMLIdentityProvider.SendSLO(context.Response, null);
    }

    public bool IsReusable
    {
      get { return false; }
    }
  }
}