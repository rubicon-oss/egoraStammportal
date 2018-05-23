/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections;
using System.Text;
using System.Web.Caching;

namespace Egora.Stammportal.HttpReverseProxy
{
  public class AuthorizationHistory
  {
    public static AuthorizationHistory[] GetHistory(Hashtable statistic, Cache cache)
    {
      ArrayList hist = new ArrayList();
      string service;
      string user;
      string app;
      foreach (string key in statistic.Keys)
      {
        AuthorizationWebServiceProxy.UnKey(key, out service, out app, out user);
        AuthorizationHistory auth = new AuthorizationHistory(service, app, user,
                                                             (CustomAuthorization) cache[key]);
        auth.Hits = (int) statistic[key];
        hist.Add(auth);
      }
      return (AuthorizationHistory[]) hist.ToArray(typeof (AuthorizationHistory));
    }

    public AuthorizationHistory(string authorizationWebService, string applicationId, string userId,
                                CustomAuthorization authorization)
    {
      _user = userId;
      _applicationId = applicationId;
      if (authorization != null)
      {
        string info;
        if (authorization.SoapHeaderXmlFragment != null)
        {
          info = authorization.SoapHeaderXmlFragment.OuterXml;
        }
        else
        {
          StringBuilder sb = new StringBuilder();
          if (authorization.HttpHeaders != null)
          {
            foreach (HttpHeader header in authorization.HttpHeaders)
            {
              if (header != null)
                sb.Append(String.Format("{0}={1}; ", header.Name, header.Value));
            }
          }
          info = sb.ToString();
        }
        _authorization = info + " TimeToLive=" + authorization.TimeToLive.ToString();
      }
    }

    private string _user;

    public string User
    {
      get { return _user; }
    }

    private string _applicationId;

    public string ApplicationId
    {
      get { return _applicationId; }
    }

    //private string _authorizationWebService;
    //public string AuthorizationWebService
    //{
    //  get { return AuthorizationWebService; }
    //}

    private int _hits;

    public int Hits
    {
      get { return _hits; }
      set { _hits = value; }
    }

    private string _authorization;

    public string Authorization
    {
      get { return _authorization; }
    }
  }
}