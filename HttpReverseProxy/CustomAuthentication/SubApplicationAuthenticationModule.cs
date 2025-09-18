using System;
using System.Web;
using System.Web.Security;
using Egora.Stammportal.Authentication;

namespace Egora.Stammportal.HttpReverseProxy.CustomAuthentication
{
  public class SubApplicationAuthenticationModule : CustomAuthenticationModuleBase
  {
    private const string Flag = "12569537-1CFF-47C2-9C18-C46C2C52C72C";
    public SubApplicationAuthenticationModule()
    {
    }

    protected override void DoAuthentication(HttpContext context)
    {
      var absoluteLoginUrl = FormsAuthentication.LoginUrl.StartsWith("/") ? 
        FormsAuthentication.LoginUrl 
        : context.Request.ApplicationPath + "/" + FormsAuthentication.LoginUrl;
      var isLoginUrl = context.Request.Url.AbsolutePath.StartsWith(absoluteLoginUrl);
      if (isLoginUrl)
      {
        // I should not receive this, should go to SubApplication 
        context.Response.StatusCode = 503;
        context.Response.StatusDescription = $"LoginUrl received";
        context.Response.End();
        return;
      }
      var authInfo = AuthenticationInformation.GetAndClearAuthenticationInformation(context);
      if (authInfo == null)
      {
        if (context.Request.Url.Query.Contains(Flag))
        {
          context.Response.StatusCode = 403;
          context.Response.StatusDescription =
            $"No AuthenticationInformation from cookie {AuthenticationInformation.GetCookieName()}";
          context.Response.End();
          return;
        }

        FormsAuthentication.RedirectToLoginPage(Flag);
        context.Response.End();
        return;
      }
      var userData = $"SecClass{authInfo.SecClass}{(authInfo.IsAdmin ? ";Admin" : "")}";

      SetPrincipalAndCookie(context, authInfo.UserName, userData);
    }
  }

}