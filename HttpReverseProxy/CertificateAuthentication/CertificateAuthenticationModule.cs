using Egora.Stammportal.HttpReverseProxy.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Xml.Serialization;
using Egora.Stammportal.HttpReverseProxy.Properties;

namespace Egora.Stammportal.HttpReverseProxy.CertificateAuthentication
{
  public class CertificateAuthenticationModule : IHttpModule
  {
    protected static CertificateAuthenticationConfiguration Config;
    public virtual void Dispose()
    {
    }

    public virtual void Init(HttpApplication application)
    {
      application.AuthenticateRequest += OnAuthenticateRequest;
      application.EndRequest += OnEndRequest;
      var filePath = Settings.Default.CertificateUserMapping;
      ReadConfiguration(filePath);
    }

    protected virtual void ReadConfiguration(string filePath)
    {
      Config = CertificateAuthenticationConfiguration.CreateFromFile(filePath);
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

      var certString = request.Headers[Config.HeaderName];
      if (certString == null)
      {
        context.Response.StatusCode = 403;
        context.Response.SubStatusCode = 7; // no valid cert 
        context.Response.StatusDescription = "No Cert found.";
        context.Response.End();
        return;
      }

      try
      {
        var certBinary = Convert.FromBase64String(certString);
        var cert = new X509Certificate2(certBinary);
        if (!cert.Verify())
        {
          context.Response.StatusCode = 403;
          context.Response.SubStatusCode = 7; // no valid cert 
          context.Response.StatusDescription = "Certificate is not valid.";
          context.Response.End();
          return;
        }

        var (userName, isAdmin) = Config.GetUsername(cert.Thumbprint, cert.Subject, cert.Issuer);
        if (userName == null)
        {
          context.Response.StatusCode = 403;
          context.Response.SubStatusCode = 7; // no valid cert 
          context.Response.StatusDescription =
            $"No mapping found for certificate thumbprint={cert.Thumbprint}, subject={cert.Subject}, issuer={cert.Issuer}.";
          context.Response.End();
          return;
        }

        userName = EvaluateUserName(context.Request.Headers, userName);

        SetPrincipalAndCookie(context, userName, isAdmin);
      }
      catch (Exception ex)
      {
        context.Response.StatusCode = 403;
        context.Response.StatusDescription = ex.Message;
        context.Response.End();
        return;
      }
    }

    protected virtual string EvaluateUserName(NameValueCollection headers, string userName)
    {
      if (userName.Substring(0, 7) == "$[HTTP_" && userName.Substring(userName.Length - 1, 1) == "]")
      {
        userName = headers[userName.Substring(7, userName.Length - 8)];
      }

      return userName;
    }


    protected void SetPrincipalAndCookie(HttpContext context, string userName, bool isAdmin)
    {
      string userData = isAdmin ? "Admin" : null;
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
      var principal = new GenericPrincipal(new FormsIdentity(ticket), ticket.UserData == null ? null : new string[] { ticket.UserData });
      return principal;
    }

    protected void OnEndRequest(object sender, EventArgs e)
    {
    }

  }

}