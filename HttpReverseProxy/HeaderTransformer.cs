/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using Egora.Stammportal.HttpReverseProxy.Mapping;
using Egora.Stammportal.HttpReverseProxy.Properties;

namespace Egora.Stammportal.HttpReverseProxy
{
  public class HeaderTransformer
  {
    #region HeaderList

    //public static List<string> KnownHeaders = new List<string> (new string[] { 
    //    "Cache-Control", "Connection", "Date", "Keep-Alive", "Pragma", "Trailer", 
    //    "Transfer-Encoding", "Upgrade", "Via", "Warning", "Allow", "Content-Length", 
    //    "Content-Type", "Content-Encoding", "Content-Language", "Content-Location", 
    //    "Content-MD5", "Content-Range", "Expires", "Last-Modified", "Accept", "Accept-Charset", 
    //    "Accept-Encoding", "Accept-Language", "Authorization", "Cookie", "Expect", "From", "Host", 
    //    "If-Match", "If-Modified-Since", "If-None-Match", "If-Range", "If-Unmodified-Since", 
    //    "Max-Forwards", "Proxy-Authorization", "Referer", "Range", "Te", "Translate", "User-Agent"
    //   });

    //public static List<string> RestrictedHeaders = new List<string> (new string[] {
    //    "Accept", "Connection", "Content-Length", "Content-Type", "Date", 
    //    "Expect", "Host", "If-Modified-Since", "Range", "Referer", "Transfer-Encoding", 
    //    "User-Agent", "Proxy-Connection"});


    //public static bool IsKnownHeader (string headerName)
    //{
    //  return KnownHeaders.Contains (headerName);
    //}

    //public static bool IsRestrictedHeader (string headerName)
    //{
    //  return WebHeaderCollection.IsRestricted (headerName);
    //}

    //static void HeaderInfoTable ()
    //{
    //  HeaderInfoTable.UnknownHeaderInfo = new HeaderInfo (string.Empty, false, false, false, HeaderInfoTable.SingleParser);
    //  HeaderInfoTable.SingleParser = new HeaderParser (HeaderInfoTable.ParseSingleValue);
    //  HeaderInfoTable.MultiParser = new HeaderParser (HeaderInfoTable.ParseMultiValue);
    //  HeaderInfo[] infoArray1 = new HeaderInfo[] { 
    //        new HeaderInfo("Age", false, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Allow", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Accept", true, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Authorization", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Accept-Ranges", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Accept-Charset", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Accept-Encoding", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Accept-Language", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Cookie", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Connection", true, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Content-MD5", false, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Content-Type", true, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Cache-Control", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Content-Range", false, false, false, HeaderInfoTable.SingleParser), 
    //      new HeaderInfo("Content-Length", true, true, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Content-Encoding", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Content-Language", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Content-Location", false, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Date", true, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("ETag", false, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Expect", true, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Expires", false, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("From", false, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Host", true, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("If-Match", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("If-Range", false, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("If-None-Match", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("If-Modified-Since", true, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("If-Unmodified-Since", false, false, false, HeaderInfoTable.SingleParser), 
    //      new HeaderInfo("Keep-Alive", false, true, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Location", false, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Last-Modified", false, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Max-Forwards", false, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Pragma", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Proxy-Authenticate", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Proxy-Authorization", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Proxy-Connection", true, false, true, HeaderInfoTable.MultiParser), 
    //      new HeaderInfo("Range", true, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Referer", true, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Retry-After", false, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Server", false, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Set-Cookie", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Set-Cookie2", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("TE", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Trailer", false, false, true, HeaderInfoTable.MultiParser), 
    //      new HeaderInfo("Transfer-Encoding", true, true, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Upgrade", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("User-Agent", true, false, false, HeaderInfoTable.SingleParser), 
    //        new HeaderInfo("Via", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Vary", false, false, true, HeaderInfoTable.MultiParser), 
    //        new HeaderInfo("Warning", false, false, true, HeaderInfoTable.MultiParser),  
    //      new HeaderInfo("WWW-Authenticate", false, true, true, HeaderInfoTable.SingleParser)
    //   };
    //  HeaderInfoTable.HeaderHashTable = new Hashtable (infoArray1.Length * 2, CaseInsensitiveAscii.StaticInstance);
    //  for (int num1 = 0; num1 < infoArray1.Length; num1++)
    //  {
    //    HeaderInfoTable.HeaderHashTable[infoArray1[num1].HeaderName] = infoArray1[num1];
    //  }
    //}

    #endregion

    public enum Event
    {
      TransformRequest = 400,
      TransformResponse,
    }

    public HeaderTransformer(HttpRequest leftSideRequest, HttpWebRequest rightSideRequest, PvpTokenHandling pvpInformationHandling, string targetRootUrl, string remoteApplicationProxyPath, bool isolateCookies, string pvpVersion)
    {
      if (leftSideRequest == null)
        throw new ArgumentException("leftSideRequest must not be null.");
      if (rightSideRequest == null)
        throw new ArgumentException("rightSideRequest must not be null.");

      if (targetRootUrl == null)
        throw new ArgumentException("targetRootUrl must not be null.");

      _leftSideRequest = leftSideRequest;
      _rightSideRequest = rightSideRequest;
      _pvpInformationHandling = pvpInformationHandling;
      _remoteApplicationProxyPath = remoteApplicationProxyPath;
      _targetRootUrl = targetRootUrl;
      _isolateCookies = isolateCookies;
      _pvpVersion = pvpVersion;
    }

    public HeaderTransformer(HttpWebResponse rightSideResponse, HttpResponse leftSideResponse, string targetRootUrl, string remoteApplicationProxyPath, bool isolateCookies)
    {
      if (rightSideResponse == null)
        throw new ArgumentException("rightSideResponse must not be null.");
      if (leftSideResponse == null)
        throw new ArgumentException("leftSideResponse must not be null.");
      if (targetRootUrl == null)
        throw new ArgumentException("targetUrlBasePath must not be null.");
      if (remoteApplicationProxyPath == null)
        throw new ArgumentException("remoteApplicationProxyPath must not be null.");

      _rightSideResponse = rightSideResponse;
      _leftSideResponse = leftSideResponse;
      _remoteApplicationProxyPath = remoteApplicationProxyPath;
      _targetRootUrl = targetRootUrl;
      _isolateCookies = isolateCookies;
    }

    private HttpRequest _leftSideRequest;
    private HttpWebRequest _rightSideRequest;
    private HttpWebResponse _rightSideResponse;
    private HttpResponse _leftSideResponse;
    private string _remoteApplicationProxyPath;
    private string _targetRootUrl;
    private PathTransformer _locationTransformer = null;

    private static string[] s_authorizationToRemove = Settings.Default.RemoveAuthorizationHeader == null
                                                        ? null
                                                        : Settings.Default.RemoveAuthorizationHeader.Split(' ');
    private static List<string> s_headersToRemove = new List<string>(Settings.Default.RequestHeaderToRemove.ToLowerInvariant().Split(' '));
    private PvpTokenHandling _pvpInformationHandling;
    private string _pvpVersion;
    private bool _isolateCookies;

    protected PathTransformer LocationTransformer
    {
      get
      {
        if (_locationTransformer == null)
          _locationTransformer = new PathTransformer(_targetRootUrl, _remoteApplicationProxyPath);
        return _locationTransformer;
      }
    }

    public virtual void Transform()
    {
      if (_leftSideRequest != null && _rightSideRequest != null)
      {
        _rightSideRequest.ServicePoint.Expect100Continue = false;
        TransformRequestHeaders(_leftSideRequest.Headers, _rightSideRequest.Headers);
      }
      else if (_rightSideResponse != null && _leftSideResponse != null && _remoteApplicationProxyPath != null)
      {
        TransformResponseHeaders();
      }
      else
      {
        throw new ApplicationException("Both Requests or both Responses must not be null.");
      }
    }

    protected virtual void TransformResponseHeaders()
    {
      var cookiesWithEmptyPath = new List<string>();
      for (int i = 0; i < _rightSideResponse.Headers.Count; i++)
      //foreach (string headerName in _rightSideResponse.Headers)
      {
        var headerName = _rightSideResponse.Headers.GetKey(i);
        var headerValues = _rightSideResponse.Headers.GetValues(i);
        foreach (var headerValue in headerValues.Length > 0 ? headerValues : new string[] {null})
        {
          switch (headerName.ToLowerInvariant())
          {
            // Do nothing
            case "keep-alive":
            case "transfer-encoding":
            case "content-length":
            case "range":
              TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose,
                (int) Event.TransformResponse,
                "Ignoring Header {0} with value {1}.", headerName,
                headerValue);
              break;

            // Cookies
            case "set-cookie":
            case "set-cookie2":
              TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose,
                (int) Event.TransformResponse,
                "Transforming Cookies.");
              var cookieName = GetCookieName(headerValue);
              if (cookieName == null)
                break;
              var hasPath = headerValue.IndexOf("path=", StringComparison.InvariantCultureIgnoreCase);
              if (hasPath == -1)
              {
                cookiesWithEmptyPath.Add(cookieName);
              }
              break;

            //Location
            case "location":
              string location = LocationTransformer.AdjustPath(headerValue);
              TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose,
                (int) Event.TransformResponse,
                "Transforming Location Header from {0} to {1}.",
                headerValue, location);
              _leftSideResponse.AddHeader(headerName, location);
              break;

            // all other Headers not handled yet are copied 1:1
            default:
              TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose,
                (int) Event.TransformResponse,
                "Copy Header {0} with value {1}.", headerName,
                headerValue);
              _leftSideResponse.AddHeader(headerName, headerValue);
              break;
          }
        }
      }
      CookieTransformer cookieTransformer = new CookieTransformer(_isolateCookies, _targetRootUrl, _rightSideResponse.ResponseUri);
      // https://github.com/dotnet/corefx/issues/19166
      // wenn Cookie Path empty, dann sollte current Path herangezogen werden.
      // Beispiel Url = https://pamgate2.portal.at/at.gv.bmvit.uhs_p/uhs_c/f?p=blablabla
      // Path sollte sein https://pamgate2.portal.at/at.gv.bmvit.uhs_p/uhs_c
      // Path ist aber https://pamgate2.portal.at/at.gv.bmvit.uhs_p/uhs_c/f
      // daher wird Cookie nicht zurückgeliefert
      foreach (HttpCookie cookie in cookieTransformer.GetLeftSideResponseCookies(_rightSideResponse.Cookies, cookiesWithEmptyPath))
        _leftSideResponse.Cookies.Add(cookie);
    }

    private string GetCookieName(string headerValue)
    {
      if (headerValue == null)
        return null;

      var indexOfEquals = headerValue.IndexOf("=");
      if (indexOfEquals < 1)
        return null;

      var cookieName = headerValue.Substring(0, indexOfEquals);
      return cookieName;
    }

    protected virtual void TransformRequestHeaders(NameValueCollection leftSideHeaders, WebHeaderCollection rightSideHeaders)
    {

      foreach (string headerName in leftSideHeaders)
      {

        var h = headerName.ToLowerInvariant();

        string headerValue = leftSideHeaders[headerName];

        if (HandlePvpHeader(headerName, headerValue, rightSideHeaders)) 
          continue;

          if (s_headersToRemove.Contains(h))
          {
            TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose,
                            (int)Event.TransformRequest,
                            "Removing Header {0}.", headerName);
            continue;
          }

        switch (h)
        {
          case "connection":
          case "date":
          case "host":
            //Restricted Header
            TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose,
                                         (int) Event.TransformRequest,
                                         "Ignoring Header {0}.", headerName);
            break;

          case "authorization":
            bool remove = false;
            if (s_authorizationToRemove != null)
            {
              foreach (string authorizationType in s_authorizationToRemove)
              {
                if (!String.IsNullOrEmpty(authorizationType) &&
                    headerValue.StartsWith(authorizationType))
                {
                  remove = true;
                  break;
                }
              }
            }

            if (!remove)
              rightSideHeaders.Add(headerName, headerValue);

            break;

          case "keep-alive":
            // true is default _rightSideRequest.KeepAlive = true;
            break;

          case "transfer-encoding":
            _rightSideRequest.TransferEncoding = headerValue;
            break;

          case "content-length":
            _rightSideRequest.ContentLength = _leftSideRequest.ContentLength;
            break;

          case "content-type":
            _rightSideRequest.ContentType = _leftSideRequest.ContentType;
            break;

          case "accept":
            _rightSideRequest.Accept = headerValue;
            break;

          case "cookie":
            TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose,
                                         (int) Event.TransformRequest, "Transforming Cookies");
            CookieTransformer cookieTransformer = new CookieTransformer(_isolateCookies, _targetRootUrl );
            _rightSideRequest.CookieContainer.Add(cookieTransformer.GetRightSideRequestCookies(_leftSideRequest));
            break;

          case "expect":
            string val = headerValue;
            if (val.ToLowerInvariant() == "100-continue")
            {
              _rightSideRequest.ServicePoint.Expect100Continue = true;
            }
            else
            {
              _rightSideRequest.Expect = headerValue;
            }
            break;

          case "if-modified-since":
            _rightSideRequest.IfModifiedSince = DateTime.Parse(headerValue);
            break;

          case "referer":
            _rightSideRequest.Referer = headerValue;
            break;

          case "range":
            string rangeValue = headerValue;
            string[] rangeParts = rangeValue.Split('=');
            string rangeType;
            if (rangeParts.Length >= 2)
            {
              rangeType = rangeParts[0];
              foreach (string range in rangeParts[1].Split(','))
              {
                string[] limits = range.Split('-');
                if (limits.Length >= 2)
                {
                  TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose, (int) Event.TransformRequest,
                                               "Transforming Range Header {0}.", range);
                  if (limits[0].Length > 0 && limits[1].Length > 0)
                    _rightSideRequest.AddRange(rangeType, Int32.Parse(limits[0]), Int32.Parse(limits[1]));
                  else if (limits[0].Length > 0 && limits[1].Length == 0)
                    _rightSideRequest.AddRange(Int32.Parse(limits[0])*(-1));
                  else if (limits[0].Length == 0 && limits[1].Length > 0)
                    _rightSideRequest.AddRange(Int32.Parse(limits[1]));
                  else
                  {
                    TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose,
                                                 (int) Event.TransformRequest, "Error Transforming Range Header {0}.",
                                                 range);
                    throw new HttpParseException("range header not understood: " + rangeValue);
                  }
                }
                else
                {
                  TraceScope.Current.TraceEvent(System.Diagnostics.TraceEventType.Verbose,
                    (int)Event.TransformRequest, "Error Transforming Range Header {0}.", range);
                  throw new HttpParseException("range header not understood: " + rangeValue);
                }
              }
            }
            break;

          case "user-agent":
            _rightSideRequest.UserAgent = _leftSideRequest.UserAgent;
            break;

          case "from":
            if (!Settings.Default.UseFromHeader)
              rightSideHeaders.Add(headerName, headerValue);
            break;

          // all other Headers not handled yet are copied 1:1
          default:
            rightSideHeaders.Add(headerName, headerValue);
            break;
        }
      }
    }

    protected bool HandlePvpHeader(string headerName, string headerValue, WebHeaderCollection rightsideHeaders)
    {
      bool isPvpHeader = false;
      string h = headerName.ToLowerInvariant();
      switch (h)
      {
        case "x-version":
        case "x-authenticate-participantid":
        case "x-authenticate-gvoudomain":
        case "x-authenticate-userid":
        case "x-authenticate-cn":
        case "x-authenticate-gvgid":
        case "x-authenticate-gvbpk":
        case "x-authenticate-gvouid":
        case "x-authenticate-gvouokz":
        case "x-authenticate-ou":
        case "x-authenticate-gvsecclass":
        case "x-authenticate-gvfunction":
        case "x-authenticate-mail":
        case "x-authenticate-tel":
        case "x-authorize-gvouid":
        case "x-authorize-ou":
        case "x-authorize-roles":
        case "x-accounting-costcenterid":
        case "x-accounting-chargecode":
        case "x-accounting-invoicerecptid":
          isPvpHeader = true;
          break;
      }

      if (!isPvpHeader && h.StartsWith("x-pvp-"))
        isPvpHeader = true;

      // ToDo Chaining over 2 or more hops
      if (isPvpHeader)
      {
        switch (_pvpInformationHandling)
        {
          case PvpTokenHandling.useConfigSetting:
            if (!Properties.Settings.Default.RemoveLeftSideAuthorization)
              rightsideHeaders.Add(headerName, headerValue);
            break;

          case PvpTokenHandling.chain:
            if (_pvpVersion == "1.8" || _pvpVersion == "1.9")
              rightsideHeaders.Add("X-01-" + headerName.Remove(0, 2), headerValue);
              // e.g. X-01-AUTHENTICATE-userId
            else if (_pvpVersion == "2.0" || _pvpVersion == "2.1")
            {
              if (h != "x-pvp-version" && h != "x-pvp-egovtoken-version")
                rightsideHeaders.Add(headerName + "_01", headerValue);
            }
            break;

          case PvpTokenHandling.remove:
            break;

          case PvpTokenHandling.preserve:
            rightsideHeaders.Add(headerName, headerValue);
            break;
        }
        return true;
      }
      return false;
    }
  }
}