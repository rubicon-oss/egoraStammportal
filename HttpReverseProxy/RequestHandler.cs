/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;
using Egora.Pvp;
using Egora.Stammportal.HttpReverseProxy.Mapping;
using Egora.Stammportal.HttpReverseProxy.Properties;
using Egora.Stammportal.HttpReverseProxy.StreamFilter;
using Directory = System.IO.Directory;

namespace Egora.Stammportal.HttpReverseProxy
{
  public class RequestHandler
  {
    public static Dictionary<PvpAttributes, string> s_ProxyHeaderNames19 = new Dictionary<PvpAttributes, string>()
                                                                       {
                                                                         { PvpAttributes.ORIG_SCHEME, "X-ORIG-SCHEME" },
                                                                         { PvpAttributes.ORIG_HOST, "X-ORIG-HOSTINFO" }, 
                                                                         {PvpAttributes.ORIG_URI, "X-ORIG-URI"},
                                                                         {PvpAttributes.TXID, "X-TXID"}
                                                                       };
    public static Dictionary<PvpAttributes, string> s_ProxyHeaderNames20 = new Dictionary<PvpAttributes, string>()
                                                                       {
                                                                         { PvpAttributes.ORIG_SCHEME, "X-PVP-ORIG-SCHEME" },
                                                                         { PvpAttributes.ORIG_HOST, "X-PVP-ORIG-HOST" }, 
                                                                         {PvpAttributes.ORIG_URI, "X-PVP-ORIG-URI"},
                                                                         {PvpAttributes.TXID, "X-PVP-TXID"}
                                                                       };

    private HttpRequest _leftSideRequest;
    private HttpWebRequest _rightSideRequest;
    private RemoteApplication _application;
    private CustomAuthorization _authorization;
    private Authentication _auth;
    private TrafficLogger _logger;

    public RequestHandler(HttpRequest leftSideRequest, RemoteApplication application, TrafficLogger logger)
    {
      _leftSideRequest = leftSideRequest;
      _application = application;
      _auth = new Authentication(_leftSideRequest);
      _logger = logger;
    }

    public virtual HttpWebRequest CreateRightSideRequest(Stream inputBuffer)
    {
      _rightSideRequest = (HttpWebRequest)HttpWebRequest.Create(_application.GetRightSideUrl(_leftSideRequest));

      _rightSideRequest.AllowAutoRedirect = false;
      _rightSideRequest.Timeout = Settings.Default.RequestTimeoutSeconds * 1000;

      _rightSideRequest.CookieContainer = new CookieContainer();

      if (_application.Certificate != null)
        _rightSideRequest.ClientCertificates.Add(_application.Certificate);

      if (!_application.ByPass(_leftSideRequest.Url.AbsolutePath))
      {
        AuthorizationWebServiceProxy authorizationProxy =
          new AuthorizationWebServiceProxy(_application.Directory.AuthorizationWebService);
        string userName = _auth.UserId;
        _authorization = authorizationProxy.GetAuthorization(_application.RootUrl, userName);

        if (!Properties.Settings.Default.ProcessRequestWithoutAuthorization)
        {
          if (_authorization == null || _authorization == CustomAuthorization.NoAuthorization)
          {
            throw new AuthorizationException("No Authorization received.");
          }
        }
      }
      HeaderTransformer headerTransformer = new HeaderTransformer(_leftSideRequest,
          _rightSideRequest,
          IsSoap ? PvpTokenHandling.remove : _application.PvpInformationHandling,
          _application.RootUrl,
          _application.RemoteApplicationProxyPath,
          _application.IsolateCookies,
          _authorization == null ? null : _authorization.PvpVersion);

      headerTransformer.Transform();

      Dictionary<PvpAttributes, string> headersNames = null;

      if (_authorization != null && _authorization.PvpVersion == PvpVersionNumber.Version19)
        headersNames = s_ProxyHeaderNames19;

      if (_authorization != null && (_authorization.PvpVersion == PvpVersionNumber.Version20 || _authorization.PvpVersion == PvpVersionNumber.Version21))
        headersNames = s_ProxyHeaderNames20;

      if (!IsSoap && headersNames != null)
      {
        _rightSideRequest.Headers.Add(headersNames[PvpAttributes.ORIG_SCHEME], _leftSideRequest.Url.Scheme);
        int port = _leftSideRequest.Url.Port;
        string portString = (port == 80 || port == 443) ? String.Empty : ":" + port.ToString();
        _rightSideRequest.Headers.Add(headersNames[PvpAttributes.ORIG_HOST], _leftSideRequest.Url.Host + portString);
        _rightSideRequest.Headers.Add(headersNames[PvpAttributes.ORIG_URI], _leftSideRequest.Url.AbsolutePath);
      }

      if (headersNames != null && String.IsNullOrEmpty(_leftSideRequest.Headers[headersNames[PvpAttributes.TXID]]))
        _rightSideRequest.Headers.Add(headersNames[PvpAttributes.TXID], GetTxId());

      _rightSideRequest.Method = _leftSideRequest.HttpMethod;

      _rightSideRequest.AuthenticationLevel =
        Egora.Stammportal.HttpReverseProxy.Properties.Settings.Default.AuthenticationLevel;
      _rightSideRequest.UseDefaultCredentials = true;
      if (_rightSideRequest.Proxy != null)
        _rightSideRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
      _rightSideRequest.PreAuthenticate = true;

      if (_authorization != null && _authorization.HttpHeaders != null &&
          _authorization != CustomAuthorization.NoAuthorization)
      {
        foreach (HttpHeader header in _authorization.HttpHeaders)
          if (header != null)
            _rightSideRequest.Headers.Add(header.Name, header.Value);
      }

      HandleRequestContent(inputBuffer);

      return _rightSideRequest;
    }

    private string GetTxId()
    {
      string firstPart = DateTime.Now.ToString("yyyyMMdd_HHmmsszz");
      return String.Format("{0}${1}@{2}", firstPart, GetNumber(firstPart), _leftSideRequest.Url.Host);
    }

    private static string s_lastToken;
    private static int s_lastNumber;

    public static string GetNumber(string token)
    {
      if (s_lastToken != token)
      {
        s_lastNumber = 0;
        s_lastToken = token;
      }
      return s_lastNumber++.ToString("0000");
    }

    public bool IsSoap
    {
      get { return _authorization != null && _authorization.SoapHeaderXmlFragment != null; }
    }

    public virtual long HandleRequestContent(Stream inputBuffer)
    {
      long length = 0;

      string method = _leftSideRequest.HttpMethod.ToUpper();
      if (_authorization == null || (_authorization != null && _authorization.SoapHeaderXmlFragment == null))
      {
        if (method == "POST" || method == "PUT")
        {
          StreamFilterBase filter = new CopyFilter(_leftSideRequest.ContentLength);
          _rightSideRequest.ContentLength = _leftSideRequest.ContentLength;

          Stream requestStream = GetRequestStream();
          if (_application.LogTraffic)
          {
            MemoryStream buffer = new MemoryStream((int)_rightSideRequest.ContentLength);
            length = filter.FilterStream(inputBuffer, buffer);
            _logger.LogRequestContent(buffer);
            buffer.WriteTo(requestStream);
          }
          else
          {
            length = filter.FilterStream(inputBuffer, requestStream);
          }
        }
      }
      else
      {
        if (method == "POST" || method == "M-POST")
        {
          MemoryStream buffer = new MemoryStream();
          var secExtNS = _application.Directory.SecExtNamespace;
          SoapFilter filter = new SoapFilter(_authorization, _leftSideRequest.ContentLength, _application.PvpInformationHandling, _leftSideRequest.Headers, secExtNS);
          length = filter.FilterStream(inputBuffer, buffer);
          _rightSideRequest.ContentLength = length;
          if (_application.LogTraffic)
            _logger.LogRequestContent(buffer);
          buffer.WriteTo(GetRequestStream());
        }
      }

      return length;
    }

    private Stream GetRequestStream()
    {
      return _auth.GetRequestStream(_rightSideRequest);
    }
  }
}