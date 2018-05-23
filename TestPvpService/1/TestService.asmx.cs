/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;
using System.ComponentModel;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace TestPvpService
{
  [System.Xml.Serialization.XmlType(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
  [System.Xml.Serialization.XmlRoot(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", IsNullable = false)]
  public class Security : System.Web.Services.Protocols.SoapHeader
  {
    /// <remarks/>
    [System.Xml.Serialization.XmlAnyElement()] public System.Xml.XmlElement pvpToken;
  }

  /// <summary>
  /// Summary description for Service1
  /// </summary>
  [WebService(Namespace = "http://tempuri.org/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem(false)]
  public class TestService : System.Web.Services.WebService
  {
    public Security PvpSecurity;

    [WebMethod]
    [SoapHeader("PvpSecurity", Direction = SoapHeaderDirection.In)]
    public string DoSomeThing()
    {
      string echo = "No header sent.";
      if (PvpSecurity != null && PvpSecurity.pvpToken != null)
        echo = PvpSecurity.pvpToken.OuterXml;
      return echo;
    }

    [WebMethod]
    [SoapHeader("PvpSecurity", Direction = SoapHeaderDirection.In)]
    public string DoError()
    {
      HttpContext.Current.Response.Write("<sometag>somecontent</sometag>");
      throw new ApplicationException("The error occured.");
    }
  }
}