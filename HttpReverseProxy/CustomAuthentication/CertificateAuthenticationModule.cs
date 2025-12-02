using System;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Egora.Stammportal.HttpReverseProxy.Properties;

namespace Egora.Stammportal.HttpReverseProxy.CustomAuthentication
{
  public class CertificateAuthenticationModule : CustomAuthenticationModuleBase
  {
    protected static CertificateAuthenticationConfiguration Config;

    public CertificateAuthenticationModule()
    {
      var filePath = Settings.Default.CertificateUserMapping;
      ReadConfiguration(filePath);
    }
    protected virtual void ReadConfiguration(string filePath)
    {
      Config = CertificateAuthenticationConfiguration.CreateFromFile(filePath);
    }

    protected override void DoAuthentication(HttpContext context)
    {
      HttpRequest request = context.Request;
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

        var (userName, isAdmin, secClass) = Config.GetUserInfo(cert.Thumbprint, cert.Subject, cert.Issuer);
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

        SetPrincipalAndCookie(context, userName, $"SecClass{secClass}" + (isAdmin ? ";Admin" : ""));
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
      if (userName.StartsWith("$[HTTP_") && userName.EndsWith("]"))
      {
        userName = headers[userName.Substring(7, userName.Length - 8)];
      }

      return userName;
    }
  }

}