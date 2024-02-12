using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Egora.Stammportal.Authentication
{
  public class AuthenticationInformation
  {
    public int SecClass;
    public string UserName;

    public HttpCookie ToCookie()
    {
      var cookieValue = ToBase64String();

      var httpCookie = new HttpCookie(Settings.Default.LoginCookieName, cookieValue);

      return httpCookie;
    }

    public override string ToString()
    {
      return $"{SecClass:D1}|{UserName}";
    }
    public string ToBase64String()
    {
      var s = Encoding.UTF8.GetBytes(ToString());
      var ticket = MachineKey.Protect(s);
      return System.Convert.ToBase64String(ticket); 
    }

    public static AuthenticationInformation FromCookie()
    {
      var cookie = HttpContext.Current.Request?.Cookies[Settings.Default.LoginCookieName];
      if (cookie != null)
      {
        var cookieValue = cookie.Value;
        return FromBase64String(cookieValue);
      }

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
          if (asString.Length > 1 && int.TryParse(asString.Substring(0, 1), out var secClass))
          {
            var authInfo = new AuthenticationInformation() { SecClass = secClass, UserName = asString.Remove(0, 2) };
            return authInfo;
          }
        }
      }

      return null;
    }

    public static HttpCookie GetDeletionCookie()
    {
      var delCookie = new HttpCookie(Settings.Default.LoginCookieName);
      delCookie.Expires = new DateTime(1900, 1, 1);
      return delCookie;
    }

    public static AuthenticationInformation LoginToFormsAuthentication()
    {
      var authInfo = AuthenticationInformation.FromCookie();
      if (authInfo != null)
      {
        var userData = $"SecClass={authInfo.SecClass};";
        var ticket = new FormsAuthenticationTicket(1, authInfo.UserName, DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), false, userData);
        var value = FormsAuthentication.Encrypt(ticket);
        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, value);
        HttpContext.Current.Response.Cookies.Add(cookie);
        HttpContext.Current.Response.Cookies.Add(AuthenticationInformation.GetDeletionCookie());
      }
      return authInfo;
    }
  }
}
