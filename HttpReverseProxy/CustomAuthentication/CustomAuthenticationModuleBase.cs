using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Egora.Stammportal.HttpReverseProxy.CustomAuthentication
{
  public class CustomAuthenticationModuleBase : IHttpModule
  {
    public virtual void Dispose()
    {
    }

    public virtual void Init(HttpApplication application)
    {
      application.AuthenticateRequest += OnAuthenticateRequest;
      application.EndRequest += OnEndRequest;
    }

    protected void OnAuthenticateRequest(object sender, EventArgs e)
    {
      HttpApplication application = (HttpApplication)sender;
      HttpContext context = application.Context;
      AuthenticateRequest(context);
    }

    protected void AuthenticateRequest(HttpContext context)
    {
      var request = context.Request;
      var protectedPath = context.Request.ApplicationPath + "/" + FormsAuthentication.FormsCookiePath;
      while (protectedPath.Contains("//"))
        protectedPath = protectedPath.Replace("//", "/");
      if (!request.Path.StartsWith(protectedPath, StringComparison.InvariantCultureIgnoreCase))
        return;

      var cookie = request.Cookies[FormsAuthentication.FormsCookieName];
      if (cookie != null)
      {
        var ticket = FormsAuthentication.Decrypt(cookie.Value);
        if (ticket != null && !ticket.Expired)
        {
          context.User = GetPrincipal(ticket);

          var newTicket = FormsAuthentication.RenewTicketIfOld(ticket);
          if (newTicket != ticket)
            SetCookie(context, newTicket);

          return;
        }
      }

      DoAuthentication(context);
    }

    protected virtual void DoAuthentication(HttpContext context)
    {
      throw new NotImplementedException();
    }

    protected void SetPrincipalAndCookie(HttpContext context, string userName, string userData)
    {
      var ticket = new FormsAuthenticationTicket(1, userName, DateTime.UtcNow, DateTime.UtcNow + FormsAuthentication.Timeout,
        false, userData, FormsAuthentication.FormsCookiePath);
      
      context.User = GetPrincipal(ticket);
      SetCookie(context, ticket);
    }

    protected void SetCookie(HttpContext context, FormsAuthenticationTicket ticket)
    {
      context.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket)));
    }

    protected IPrincipal GetPrincipal(FormsAuthenticationTicket ticket)
    {
      var principal = new GenericPrincipal(new FormsIdentity(ticket), ticket.UserData == null ? null :  ticket.UserData.Split(';') );
      return principal;
    }

    protected void OnEndRequest(object sender, EventArgs e)
    {
    }
  }
}