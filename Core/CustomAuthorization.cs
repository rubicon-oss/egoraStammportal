/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Egora.Stammportal
{
  /// <summary>
  /// Holds the information the ReverseProxy adds to the right side request.
  /// </summary>
  public class CustomAuthorization
  {
    // constants

    public static CustomAuthorization NoAuthorization = new CustomAuthorization(
      new HttpHeader[]
        {new HttpHeader("91CBD7B0-1A79-4295-B224-00E442C8E7FF", "32E27178-6947-4834-8AB8-76AE16E74DD1")});

    // types

    // static members
    public const string Namespace = "http://www.egora.at/Stammportal/AuthorizationWebService/1.0";

    // member fields
    private HttpHeader[] _httpHeaders;
    private XmlElement _soapHeaderXmlFragment;
    private int _timeToLive;
    private string _version;

    // construction and disposing

    public CustomAuthorization()
    {
    }

    public CustomAuthorization(HttpHeader[] headers)
    {
      _httpHeaders = headers;
      _timeToLive = 600;
    }

    // methods and properties

    /// <summary>
    /// If the application is a webapplication, this collection holds the http-headers.
    /// </summary>
    public HttpHeader[] HttpHeaders
    {
      get { return _httpHeaders; }
      set { _httpHeaders = value; }
    }

    /// <summary>
    /// If the application is a SOAP Service, this XmlFragment holds the Pvp-Soap-Header.
    /// </summary>
    public XmlElement SoapHeaderXmlFragment
    {
      get { return _soapHeaderXmlFragment; }
      set { _soapHeaderXmlFragment = value; }
    }

    /// <summary>
    /// Time in seconds the Reverse Proxy may cache this information.
    /// </summary>
    public int TimeToLive
    {
      get { return _timeToLive; }
      set { _timeToLive = value; }
    }

    /// <summary>
    /// PvpVersion
    /// </summary>
    public string PvpVersion
    {
      get {return _version; }
      set { _version = value; }
    }

    private List<string> _secClassHeaders = new List<string>() { "X-PVP-SECCLASS", "X-AUTHENTICATE-gvSecClass" };
    /// <summary>
    /// SecClass, retrieved from Headers
    /// </summary>
    [XmlIgnore]
    public string SecClass
    {
      get
      {
        if (HttpHeaders != null)
        {
          foreach (var httpHeader in HttpHeaders)
          {
            if (httpHeader.Name.Equals("X-AUTHENTICATE-gvSecClass", StringComparison.InvariantCultureIgnoreCase) ||
                httpHeader.Name.Equals("X-PVP-SECCLASS", StringComparison.InvariantCultureIgnoreCase))
              return httpHeader.Value;
          }
        }

        if (SoapHeaderXmlFragment != null)
        {
          var nsManager = new XmlNamespaceManager(new NameTable());
          nsManager.AddNamespace("pvp", "http://egov.gv.at/pvp1.xsd");
          var secClass = SoapHeaderXmlFragment.SelectSingleNode("//pvp:gvSecClass", nsManager);
          if (secClass != null)
            return secClass.InnerText;
        }
        
        return null;
      }
    }

  }
}