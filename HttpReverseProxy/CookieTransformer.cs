/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Egora.Stammportal.HttpReverseProxy.Properties;

namespace Egora.Stammportal.HttpReverseProxy
{
  // folgende Punkt offen:
  // 1) die meisten Browser unterstützen 20 Cookies pro Domain
  // 2) wenn Secure gesetzt ist, und Client kommt mit http, dann geht Cookie verloren
  // 3) im Value werden vorne domain und path dazugenommen, dadurch kann hinten was verloren gehen
  public class CookieTransformer
  {
    public const string c_CookieSignature = "CD9C0F68-2099-4f51-A585-E9758EF0A020";
    private string _cookieNamePrefix;
    private List<string> _passThroughCookies;
    private Uri _targetUri;
    private string _defaultCookiePath;

    public CookieTransformer(bool isolateCookies, string targetRootUrl, Uri rightSideUrl, List<string> passThroughCookies)
      : this(isolateCookies, targetRootUrl, passThroughCookies)
    {
      var indexOfLastSlash = rightSideUrl.AbsolutePath.LastIndexOf("/");
      if (indexOfLastSlash == 0)
        indexOfLastSlash = 1;
      _defaultCookiePath = rightSideUrl.AbsolutePath.Substring(0, indexOfLastSlash);
    }

    public CookieTransformer(bool isolateCookies, string targetRootUrl, List<string> passThroughCookies = null)
    {
      if (targetRootUrl == null)
        throw new ArgumentNullException(nameof(targetRootUrl));

      _targetUri = new Uri(targetRootUrl);
      if (!_targetUri.IsAbsoluteUri)
        throw new ArgumentException("targetRootUrl must be an absolute Uri.");

      _cookieNamePrefix = _targetUri.Host + (_targetUri.IsDefaultPort ? String.Empty : ":" + _targetUri.Port) + (isolateCookies ? _targetUri.AbsolutePath : "/");
      _passThroughCookies = passThroughCookies ?? new List<string>();
    }
    public virtual HttpCookie[] GetLeftSideResponseCookies(CookieCollection rightSideResponseCookies, List<string> cookiesWithEmptyPath)
    {
      List<HttpCookie> leftSideResponseCookies = new List<HttpCookie>();

      foreach (Cookie rightSideResponseCookie in rightSideResponseCookies)
      {
        leftSideResponseCookies.Add(CreateLeftSideResponseCookie(rightSideResponseCookie, cookiesWithEmptyPath.Contains(rightSideResponseCookie.Name)));
      }

      return leftSideResponseCookies.ToArray();
    }

    private string CookieNamePrefix
    {
      get { return _cookieNamePrefix; }
    }

    public virtual HttpCookie CreateLeftSideResponseCookie(Cookie rightSideResponseCookie, bool useRequestPath)
    {
      HttpCookie newCookie;
      if (_passThroughCookies.Contains(rightSideResponseCookie.Name, StringComparer.OrdinalIgnoreCase) && !rightSideResponseCookie.HttpOnly)
      {
        newCookie = new HttpCookie(rightSideResponseCookie.Name, rightSideResponseCookie.Value);
        newCookie.Path = rightSideResponseCookie.Path; // hopefully no collision and 1:1 mapping of urls
      }
      else
      {
        var path = Settings.Default.WorkaroundCookieWithEmptyPath && useRequestPath ? _defaultCookiePath : rightSideResponseCookie.Path;
        string value = String.Format("{0}|{1}|{2}|{3}", c_CookieSignature, rightSideResponseCookie.Domain,
          path, rightSideResponseCookie.Value);
        newCookie = new HttpCookie(CookieNamePrefix + rightSideResponseCookie.Name, value);

        if (HttpContext.Current != null && HttpContext.Current.Request != null)
          newCookie.Path = HttpContext.Current.Request.ApplicationPath;
      }
      newCookie.Expires = rightSideResponseCookie.Expires;
      newCookie.HttpOnly = rightSideResponseCookie.HttpOnly;
      newCookie.Secure = rightSideResponseCookie.Secure &&
                         (HttpContext.Current == null || HttpContext.Current.Request.IsSecureConnection);

      return newCookie;
    }

    public Cookie CreateRightSideRequestCookie(HttpCookie leftSideRequestCookie, string targetPath, bool targetSecure)
    {
      Cookie rightSideCookie;
      if (_passThroughCookies.Contains(leftSideRequestCookie.Name, StringComparer.OrdinalIgnoreCase))
      {
        rightSideCookie = new Cookie(leftSideRequestCookie.Name, leftSideRequestCookie.Value, leftSideRequestCookie.Path); //hopefully 1:1 mapping
        rightSideCookie.Domain = _targetUri.Authority;
      }
      else
      {
        string[] infos = leftSideRequestCookie.Value.Split(new char[] { '|' }, 4);
        if (infos.Length < 4)
          // not my cookie
          return null;

        string signature = infos[0];
        if (signature != c_CookieSignature)
          return null;

        if (!leftSideRequestCookie.Name.StartsWith(CookieNamePrefix))
          return null; // can not handle this cookie

        rightSideCookie = new Cookie(leftSideRequestCookie.Name.Remove(0, CookieNamePrefix.Length), infos[3]);

        if (!String.IsNullOrEmpty(infos[1]))
          rightSideCookie.Domain = infos[1];

        string cookiePath = infos[2];
        if (cookiePath != null && targetPath != null
                               && cookiePath.StartsWith(targetPath, StringComparison.InvariantCultureIgnoreCase)
                               && !cookiePath.StartsWith(targetPath, StringComparison.InvariantCulture))
          cookiePath = targetPath + cookiePath.Remove(0, targetPath.Length);

        rightSideCookie.Path = cookiePath;
      }

      rightSideCookie.Expires = leftSideRequestCookie.Expires;
      rightSideCookie.HttpOnly = leftSideRequestCookie.HttpOnly;
      rightSideCookie.Secure = leftSideRequestCookie.Secure && targetSecure;
      return rightSideCookie;
    }

    public CookieCollection GetRightSideRequestCookies(HttpRequest leftSideRequest)
    {
      CookieCollection cookies = new CookieCollection();
      foreach (string leftSideCookieName in leftSideRequest.Cookies)
      {
        HttpCookie leftSideRequestCookie = leftSideRequest.Cookies[leftSideCookieName];
        bool targetSecure = string.Compare(_targetUri.Scheme, Uri.UriSchemeHttps, StringComparison.InvariantCultureIgnoreCase) == 0;

        Cookie rightSideRequestCookie = CreateRightSideRequestCookie(leftSideRequestCookie, _targetUri.AbsolutePath, targetSecure);

        if (rightSideRequestCookie != null)
          cookies.Add(rightSideRequestCookie);
      }
      return cookies;
    }
  }
}