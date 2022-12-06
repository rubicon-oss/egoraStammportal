/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;
using Egora.Stammportal.HttpReverseProxy.Properties;

namespace Egora.Stammportal.HttpReverseProxy
{
  public class Authentication
  {
    public enum Event
    {
      RetrieveIdentity = 500,
      IsAdmin,
      GetRequestStream,
      GetResponse
    }

    private HttpRequest _leftSideRequest;

    public Authentication(HttpRequest leftSideRequest)
    {
      _leftSideRequest = leftSideRequest;
    }

    private bool _useFromHeader = Settings.Default.UseFromHeader;

    public virtual WindowsIdentity GetIdentity()
    {
      if (_leftSideRequest.LogonUserIdentity != null)
      {
        TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose, (int) Event.RetrieveIdentity,
                                     "Request.LogonUserIdentity.Name={0}",
                                     _leftSideRequest.LogonUserIdentity.Name);
        return _leftSideRequest.LogonUserIdentity;
      }
      else
      {
        TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose, (int) Event.RetrieveIdentity,
                                     "WindowsIdentity.GetCurrent().Name={0}", WindowsIdentity.GetCurrent().Name);
        return WindowsIdentity.GetCurrent();
      }
    }

    public virtual string UserId
    {
      get
      {
        if (UseFromHeader)
        {
          string from = _leftSideRequest.Headers["From"];
          if (!String.IsNullOrEmpty(from))
            return from;
        }
        else if (HttpContext.Current.User.Identity is FormsIdentity)
        {
          return HttpContext.Current.User.Identity.Name;
        }

        return GetIdentity().Name;
      }
    }

    public virtual bool IsAdmin
    {
      get
      {
        WindowsPrincipal principal = new WindowsPrincipal(_leftSideRequest.LogonUserIdentity);

        TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose, (int)Event.IsAdmin,
                                     "principal {0} IsInRole({1}) is {2}", principal.Identity.Name,
                                     Settings.Default.AdministrationGroup,
                                     principal.IsInRole(Settings.Default.AdministrationGroup));

        return principal.IsInRole(Settings.Default.AdministrationGroup);
      }
    }

    ///<summary>
    /// If true, UserId comes from HttpHeader From. Default value set by config.
    ///</summary>
    public bool UseFromHeader
    {
      get { return _useFromHeader; }
      set { _useFromHeader = value; }
    }

    public virtual Stream GetRequestStream(HttpWebRequest rightSideRequest)
    {
      Stream rightSideRequestStream;

      if (Settings.Default.ImpersonateWebRequest)
      {
        TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose, (int)Event.GetRequestStream,
                                     "Impersonating.");

        using (WindowsImpersonationContext impersonationContext = GetIdentity().Impersonate())
        {
          rightSideRequestStream = rightSideRequest.GetRequestStream();
          impersonationContext.Undo();
        }
      }
      else
      {
        rightSideRequestStream = rightSideRequest.GetRequestStream();
      }
      return rightSideRequestStream;
    }

    public virtual HttpWebResponse GetWebResponse(HttpWebRequest rightSideRequest, TraceScope traceScope)
    {
      HttpWebResponse webResponse = null;

      if (Settings.Default.ImpersonateWebRequest)
      {
        traceScope.TraceEvent(System.Diagnostics.TraceEventType.Verbose, (int) Event.GetResponse,
                                     "Impersonating.");

        using (WindowsImpersonationContext impersonationContext = GetIdentity().Impersonate())
        {
          try
          {
            webResponse = (HttpWebResponse) rightSideRequest.GetResponse();
          }
          catch (WebException webException)
          {
            if (webException.Response == null)
              throw;
            webResponse = (HttpWebResponse) webException.Response;
          }
          impersonationContext.Undo();
        }
      }
      else
      {
        try
        {
          webResponse = (HttpWebResponse) rightSideRequest.GetResponse();
        }
        catch (WebException webException)
        {
          if (webException.Response == null)
            throw;
          webResponse = (HttpWebResponse) webException.Response;
        }
      }

      return webResponse;
    }

    public void EnsureAuthentication()
    {
      var cookie = _leftSideRequest.Cookies[Settings.Default.AuthenticationCookieName];
      if (cookie != null)
      {
        var ticket = FormsAuthentication.Decrypt(cookie.Value);
        if (ticket != null && ticket.Name == UserId && !ticket.Expired)
          return;
      }

      StartCheck();
    }

    private void StartCheck()
    {
      var startPath = GetAuthenticationCheckerStartPath(_leftSideRequest);
      startPath += startPath.Contains("?") ? "&" : "?";
      startPath += "ReturnUrl=" + System.Web.HttpUtility.UrlEncode(_leftSideRequest.Url.OriginalString);
      startPath += "&UserId=" + System.Web.HttpUtility.UrlEncode(UserId);
      startPath += "&FrontEnd=" + System.Web.HttpUtility.UrlEncode(_leftSideRequest.Url.GetLeftPart(UriPartial.Authority)+_leftSideRequest.ApplicationPath);
      HttpContext.Current.Response.Redirect(startPath);
    }

    public static string GetAuthenticationCheckerStartPath(HttpRequest leftSideRequest)
    {
      return MyCombine( leftSideRequest.Url.GetLeftPart(UriPartial.Authority), leftSideRequest.ApplicationPath,  Settings.Default.AuthenticationCheckerStartPath);
    }

    public static string MyCombine(string firstPart, params string[] urlParts)
    {
      var sb = new StringBuilder(firstPart);
      
      if (urlParts == null)
        return sb.ToString();

      foreach (var part in urlParts)
      {
        if (string.IsNullOrEmpty(part))
          continue;

        if (sb[sb.Length - 1].Equals('/') && part.StartsWith("/"))
          sb.Remove(sb.Length - 1, 1);
        
        sb.Append(part);
      }
      
      return sb.ToString();
    }

    public static void CreateAuthenticationCookie(HttpResponse response, string userId, string userData)
    {
      var now = DateTime.Now;
      FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, userId, now, now + new TimeSpan(Settings.Default.AuthenticationCheckerValidHours, 0, 0), false, userData);
      var value =FormsAuthentication.Encrypt(ticket);
      response.Cookies.Add( new HttpCookie(Settings.Default.AuthenticationCookieName, value));
    }
  }
}