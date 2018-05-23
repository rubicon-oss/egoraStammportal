/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;

namespace Egora.Stammportal.HttpReverseProxy
{
  public class AuthorizationWebServiceProxy : IAuthorizationContract
  {
    public enum Event
    {
      GetAuthorization = 300,
      UsingCachedAuthorization,
    }

    private static Hashtable s_statistics;
    private static object s_initializeLock = new object();
    private static Cache s_cache;
    private static string s_signature = typeof (AuthorizationWebServiceProxy).GetHashCode().ToString();

    public static void Initialize()
    {
      Initialize(null);
    }

    public static void Initialize(Cache cache)
    {
      lock (s_initializeLock)
      {
        s_cache = cache;
        s_statistics = new Hashtable();
        System.Collections.Specialized.StringCollection keys =
          new System.Collections.Specialized.StringCollection();
        if (AuthorizationCache != null) //happens in UnitTest
        {
          foreach (DictionaryEntry item in AuthorizationCache)
          {
            string key = (string) item.Key;
            if ((key.EndsWith("|" + s_signature)) &&
                ((AuthorizationCache[key] as CustomAuthorization) != null))
              keys.Add(key);
          }
          foreach (string key in keys)
            AuthorizationCache.Remove(key);
        }
      }
    }

    public static string Key(string authorizationWebServiceUrl, string applicationRootUrl, string userId)
    {
      return authorizationWebServiceUrl + "|" + applicationRootUrl + "|" + userId + "|" + s_signature;
    }

    public static void UnKey(string key, out string authorizationWebServiceUrl, out string applicationRootUrl,
                             out string userId)
    {
      string[] s = key.Split(new char[] {'|'}, 4);
      authorizationWebServiceUrl = s[0];
      applicationRootUrl = s[1];
      userId = s[2];
    }

    private static Cache AuthorizationCache
    {
      get
      {
        if (s_cache != null)
          return s_cache;

        if (HttpContext.Current != null)
          return HttpContext.Current.Cache;

        return null;
      }
    }

    public AuthorizationWebServiceProxy(string authorizationWebServiceUrl)
    {
      _authorizationWebServiceUrl = authorizationWebServiceUrl;
    }

    private string _authorizationWebServiceUrl;
    private object _insertCacheLock = new object();

    #region IAuthorizationContract Members

    public virtual CustomAuthorization GetAuthorization(string applicationRootUrl, string userId)
    {
      string cacheKey = Key(_authorizationWebServiceUrl, applicationRootUrl, userId);
      CustomAuthorization authorization = (CustomAuthorization) AuthorizationCache[cacheKey];
      if (authorization == null)
      {
        TraceScope.Current.TraceEvent(TraceEventType.Verbose, (int) Event.GetAuthorization,
                                     "Getting Authorization for RootUrl={0}, UserName={1}", applicationRootUrl,
                                     userId);
        PvpAuthorizer authorizer = new PvpAuthorizer();
        authorizer.Url = _authorizationWebServiceUrl;
        authorization = authorizer.GetAuthorization(applicationRootUrl, userId);
        if (authorization != null && authorization.TimeToLive > 0)
        {
          lock (_insertCacheLock)
          {
            AuthorizationCache.Insert(cacheKey, authorization,
                                      null, DateTime.Now.AddSeconds(authorization.TimeToLive),
                                      Cache.NoSlidingExpiration);

            if (s_statistics[cacheKey] == null)
              s_statistics[cacheKey] = 0;
          }
        }
      }
      else
      {
        TraceScope.Current.TraceEvent(TraceEventType.Verbose, (int)Event.UsingCachedAuthorization,
                                     "Using cached Authorization for ApplicationID={0}, UserName={1}",
                                     applicationRootUrl, userId);

        int c = ((int) s_statistics[cacheKey]);
        c++;
        s_statistics[cacheKey] = c;
      }

      return authorization;
    }

    #endregion

    internal static AuthorizationHistory[] History
    {
      get { return AuthorizationHistory.GetHistory(s_statistics, AuthorizationCache); }
    }
  }
}