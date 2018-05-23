/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System.Xml;

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
    /// If tha application is a webapplication, this collection holds the http-headers.
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

  }
}