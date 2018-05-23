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
  public class PvpAuthorizer : System.Web.Services.WebService
                               , IAuthorizationContract // Implementing IAuthorizationContract is not necessary.
    // The only binding between this service and a consumer is the wsdl.
    // But if this service is implemented using .NET it is convenient to implement the interface.
  {
    #region IAuthorizationContract Members

    /// <summary>
    /// Gets the authorization information for a given application and user
    /// </summary>
    /// <param name="rootUrl">RootUrl of Application</param>
    /// <param name="userId">Unique identifier of a user, e.g. loginname</param>
    /// <returns>HttpHeaders for ApplicationType Web, SOAP Header for ApplcationType SOAP</returns>
    [WebMethod]
    public CustomAuthorization GetAuthorization(string rootUrl, string userId)
    {
      CustomAuthorization auth = null;
      string url = rootUrl.ToLowerInvariant();
      if (url == "https://portal.bmi.gv.at/portal/zmr-gw/")
      {
        auth = new CustomAuthorization();
        auth.HttpHeaders = new HttpHeader[]
                             {
                               new HttpHeader("X-Version", "1.9"),
                               new HttpHeader("X-AUTHENTICATE-participantId", "AT:VKZ:GGA-50101"),
                               new HttpHeader("X-AUTHENTICATE-UserId", @"MAGSBG\90005@MAGSBG.GV.AT"),
                               new HttpHeader("X-AUTHENTICATE-cn", ""),
                               new HttpHeader("X-AUTHENTICATE-gvGid", ""),
                               new HttpHeader("X-AUTHENTICATE-gvOuId", "AT:VKZ:GGA-50101"),
                               new HttpHeader("X-AUTHENTICATE-Ou", "Stadtgemeinde Salzburg"),
                               new HttpHeader("X-AUTHENTICATE-gv-Function", ""),
                               new HttpHeader("X-AUTHENTICATE-mail", ""),
                               new HttpHeader("X-AUTHENTICATE-tel", ""),
                               new HttpHeader("X-AUTHENTICATE-gvSecClass", "2"),
                               new HttpHeader("X-AUTHORIZE-roles", ""),
                               new HttpHeader("X-AUTHORIZE-gvOuId", ""),
                               new HttpHeader("X-AUTHORIZE-Ou", "")
                             };
        auth.TimeToLive = 600;
      }
      else if (url ==
               "https://portals-test.bmi.gv.at/bmi.gv.at/soapv2/soaphttpengine/soapv2%23pvp1?dest=zmr&opzone=test"
               ||
               url ==
               "https://portals2.bmi.gv.at/bmi.gv.at/soapv2/soaphttpengine/soapv2%23pvp1?dest=zmr&opzone=produktion")
      {
        auth = new CustomAuthorization();
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(
          @"<pvpToken version=""1.9"" xmlns=""http://egov.gv.at/pvp1.xsd"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <authenticate>
    <participantId xsi:type=""xsd:string"">BNC</participantId>
    <userPrincipal>
      <userId>werner.kugler@rubicon-it.com</userId>
      <cn>Werner Kugler</cn>
      <gvOuId>rubicon</gvOuId>
      <ou>rubicon informationstechnologie gmbh/</ou>
      <gvGid>werner.kugler@rubicon-it.com</gvGid>
    </userPrincipal>
  </authenticate>
  <authorize>
    <role value=""ZMR-Behoerdenanfrage"" />
  </authorize>
</pvpToken>");

//@"<pvpToken version=""1.9"" xmlns=""http://egov.gv.at/pvp1.xsd"">
//<authenticate>
//<participantId>BNC</participantId>
//<userPrincipal>
//<userId>werner.kugler@rubicon-it.com</userId>
//<cn>Werner Kugler</cn>
//<gvOuId>rubicon</gvOuId>
//<ou>rubicon informationstechnologie gmbh/Test</ou>
//<mail>werner.kugler@rubicon-it.com</mail>
//<tel>06766612078</tel>
//<gvSecClass>2</gvSecClass>
//</userPrincipal>
//</authenticate>
//<authorize>
//<role value=""ZMR-Fremdenbehoerdenanfrage"">
//</role>
//</authorize>
//</pvpToken>");

        auth.SoapHeaderXmlFragment = doc.DocumentElement;
        auth.TimeToLive = 600;
      }
      else if (url == "http://demo.rubicon-it.com/expvp")
      {
        auth = new CustomAuthorization();
        auth.HttpHeaders = new HttpHeader[]
                             {
                               new HttpHeader("X-Version", "1.9"),
                               new HttpHeader("X-AUTHENTICATE-ParticipantId", ""),
                               new HttpHeader("X-AUTHENTICATE-GvOuDomain", ""),
                               new HttpHeader("X-AUTHENTICATE-UserId", "rainer.hoerbe@bmi.gv.at"),
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

      return auth;
    }

    #endregion
  }
}