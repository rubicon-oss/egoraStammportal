/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System.ComponentModel;
using System.Web.Services;
using System.Xml;

namespace Egora.Stammportal.TestAuthorizationWebService
{
  /// <summary>
  /// Test implementation of an AuthorizationWebService
  /// </summary>
  [WebService(Namespace = CustomAuthorization.Namespace)]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem(false)]
  public class SampleAuthorizer : System.Web.Services.WebService
                                  , IAuthorizationContract // Implementing IAuthorizationContract is not necessary.
    // The only binding between this service and a consumer is the wsdl.
    // But if this service is implemented using .NET it is convenient to implement the interface.
  {
    #region IAuthorizationContract Members

    /// <summary>
    /// Gets the authorization information for a given application and user
    /// </summary>
    /// <param name="rootUrl">The rootUrl of the application to authorize</param>
    /// <param name="userId">Unique identifier of a user, e.g. loginname</param>
    /// <returns>HttpHeaders for a webapplication, SOAP Header for a SOAP Service</returns>
    [WebMethod]
    public CustomAuthorization GetAuthorization(string rootUrl, string userId)
    {
      CustomAuthorization auth = null;
      string url = rootUrl.ToLowerInvariant();
      //Zmr Gui
      if (url == "https://portal.bmi.gv.at/portal/zmr-gw/")
      {
        auth = new CustomAuthorization();
        // in real world this Headers have values depending on userId
        auth.HttpHeaders = new HttpHeader[]
                             {
                               new HttpHeader("X-Version", "1.9"),
                               new HttpHeader("X-AUTHENTICATE-ParticipantId", ""),
                               new HttpHeader("X-AUTHENTICATE-GvOuDomain", ""),
                               new HttpHeader("X-AUTHENTICATE-UserId", ""),
                               new HttpHeader("X-AUTHENTICATE-Cn", ""),
                               new HttpHeader("X-AUTHENTICATE-GvGId", ""),
                               new HttpHeader("X-AUTHENTICATE-GvOuId", ""),
                               new HttpHeader("X-AUTHENTICATE-Ou", ""),
                               new HttpHeader("X-AUTHENTICATE-GvFunction", ""),
                               new HttpHeader("X-AUTHENTICATE-Mail", ""),
                               new HttpHeader("X-AUTHENTICATE-Tel", ""),
                               new HttpHeader("X-AUTHORIZE-roles", "Beispielrolle()")
                             };
        auth.TimeToLive = 600;
      }

        // ZMR SOAP
      else if (url ==
               "https://portals2.bmi.gv.at/bmi.gv.at/soapv2/soaphttpengine/soapv2%23pvp1?dest=ZMR&opzone=produktion")
      {
        auth = new CustomAuthorization();
        XmlDocument doc = new XmlDocument();
        // sample from pvp documentation
        // in real world the pvp token has to be constructed from data specific to userId
        doc.LoadXml(
          @"<pvpToken version=""1.9"" xmlns=""http://egov.gv.at/pvp1.xsd"">
<authenticate>
<participantId>AT:L6:994</participantId>
<userPrincipal>
<userId>fmeier@stmk.gv.at</userId>
<cn>F. Meier</cn>
<gvOuId>AT:L6:1299</gvOuId>
<ou>L6AL-F2/c</ou>
<mail>fmeier@stmk.gv.at</mail>
<tel>fmeier@stmk.gv.at</tel>
<gvSecClass>2</gvSecClass>
<gvGid>AT:B:0:UhO5RG++klaOTsVY+CU=</gvGid>
<gvFunction>SB</gvFunction>
</userPrincipal>
</authenticate>
<authorize>
<role value=""ZMR-Fremdenbehoerdenanfrage"">
<param>
<key>GKZ</key>
<value>60100</value>
</param>
</role>
</authorize>
</pvpToken>");

        auth.SoapHeaderXmlFragment = doc.DocumentElement;
        auth.TimeToLive = 600;
      }

      return auth;
    }

    #endregion
  }
}