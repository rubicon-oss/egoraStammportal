using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Egora.Stammportal.Authentication
{
  public class AuthenticationInformation
  {
    internal static TraceSource Trace = new TraceSource("AuthenticationInformation");

    public int SecClass;
    public string UserName;
    public bool IsAdmin;

    public HttpCookie ToCookie()
    {
      var cookieValue = ToBase64String();

      var cookieName = GetCookieName();
      var httpCookie = new HttpCookie(cookieName, cookieValue);

      Trace.TraceInformation("Created cookie with name " + cookieName);
      return httpCookie;
    }

    public static string GetCookieName()
    {
      string value = ConfigurationManager.AppSettings["LoginCookieName"];
      if (string.IsNullOrEmpty(value))
        value = "StammportalLogin";
      return value;
    }
    public override string ToString()
    {
      return $"{SecClass:D1}|{(IsAdmin ? "1" : "0")}|{UserName}";
    }
    public string ToBase64String()
    {
      var s = Encoding.UTF8.GetBytes(ToString());
      var ticket = MachineKey.Protect(s);
      return System.Convert.ToBase64String(ticket);
    }

    public static AuthenticationInformation FromCookie(HttpContext context)
    {
      var cookieName = GetCookieName();
      var cookie = context.Request.Cookies[cookieName];
      if (cookie != null)
      {
        var cookieValue = cookie.Value;
        Trace.TraceInformation("Retrieving cookie with name " + cookieName + " and value " + cookieValue);
        return FromBase64String(cookieValue);
      }

      Trace.TraceEvent(TraceEventType.Error, 1, "There is no cookie with name " + cookieName);
      context.Trace.Write("There is no cookie with name " + cookieName);
      return null;
    }

    public static AuthenticationInformation FromBase64String(string base64String)
    {
      if (base64String != null)
      {
        var ticket = System.Convert.FromBase64String(base64String);
        var decryptedValue = MachineKey.Unprotect(ticket);
        if (decryptedValue != null)
        {
          var asString = Encoding.UTF8.GetString(decryptedValue);
          if (asString.Length > 3
              && int.TryParse(asString.Substring(0, 1), out var secClass)
              && int.TryParse(asString.Substring(2, 1), out var isAdmin))
          {
            var authInfo = new AuthenticationInformation()
            {
              SecClass = secClass,
              IsAdmin = isAdmin == 1,
              UserName = asString.Remove(0, 4)
            };
            return authInfo;
          }
        }
      }

      Trace.TraceEvent(TraceEventType.Error, 2, "Could not decrypt value " + base64String);
      HttpContext.Current.Trace.Write("Could not decrypt value " + base64String);
      return null;
    }

    public static HttpCookie GetDeletionCookie()
    {
      var delCookie = new HttpCookie(GetCookieName());
      delCookie.Expires = new DateTime(1900, 1, 1);
      return delCookie;
    }

    public static AuthenticationInformation GetAndClearAuthenticationInformation(HttpContext context)
    {
      var authInfo = AuthenticationInformation.FromCookie(context);
      if (authInfo != null)
      {
        HttpContext.Current.Response.Cookies.Add(AuthenticationInformation.GetDeletionCookie());
      }
      return authInfo;
    }
  }
}
