/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Serialization;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests.IntegrationTest
{
  [XmlRoot()]
  public class RequestInformation
  {
    private CookieInformation[] _cookies;
    private HeaderInformation[] _headers;

    public RequestInformation()
    {
    }

    public RequestInformation(HttpRequest request)
    {
      List<HeaderInformation> headers = new List<HeaderInformation>();
      foreach (string headerName in request.Headers)
      {
        headers.Add(new HeaderInformation(headerName, request.Headers[headerName]));
      }
      Headers = headers.ToArray();
      
      List<CookieInformation> cookies = new List<CookieInformation>();
      foreach (string cookieName in request.Cookies)
      {
        cookies.Add(new CookieInformation(request.Cookies[cookieName]));
      }
      Cookies = cookies.ToArray();
    }

    public string GetHeader(string headerName)
    {
      foreach (HeaderInformation header in Headers)
      {
        if (header.Name.Equals(headerName, StringComparison.InvariantCultureIgnoreCase))
          return header.Value;
      }

      return null;
    }

    public CookieInformation GetCookie(string cookieName)
    {
      foreach (CookieInformation cookie in Cookies)
      {
        if (cookie.Name == cookieName)
          return cookie;
      }

      return null;
    }

    [XmlElement()]
    public HeaderInformation[] Headers
    {
      get { return _headers; }
      set { _headers = value; }
    }

    [XmlElement()]
    public CookieInformation[] Cookies
    {
      get { return _cookies; }
      set { _cookies = value; }
    }
  }
}