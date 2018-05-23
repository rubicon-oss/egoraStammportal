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
using Egora.Pvp;
using Egora.Pvp.Attributes;

namespace Egora.Stammportal.TestAuthorizationWebService
{
  /// <summary>
  /// Test implementation of an AuthorizationWebService
  /// </summary>
  [WebService(Namespace = CustomAuthorization.Namespace)]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem(false)]
  public class PvpTestAuthorizer : System.Web.Services.WebService
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
      if (url == "http://egoratest/pvptestapplication/1/")
      {
        auth = new CustomAuthorization();

        auth.HttpHeaders = new HttpHeader[]
                             {
                               new HttpHeader("X-AUTHENTICATE-UserId", userId),
                               new HttpHeader("PVP-Header1", "PVP-Value1"),
                               new HttpHeader("X-Version", "1.9"),
                               new HttpHeader("X-AUTHENTICATE-cn", "common name"),
                               new HttpHeader("X-AUTHENTICATE-gvOuId", "1234"),
                               new HttpHeader("X-AUTHENTICATE-ou", "egora"),
                               new HttpHeader("X_AUTHENTICATE_participantId", "test"),
                             };
        auth.PvpVersion = "1.9";
        auth.TimeToLive = 600;
      }

      else if (url == "http://egoratest/pvptestapplication/2/")
      {
        auth = new CustomAuthorization();

        auth.HttpHeaders = new HttpHeader[]
                             {
                               new HttpHeader("X-PVP-UserId", userId),
                               new HttpHeader("PVP-Header1", "PVP-Value1"),
                               new HttpHeader("X-PVP-Version", "2.1"),
                               new HttpHeader("X-PVP-PRINCIPAL-NAME", "common name"),
                               new HttpHeader("X-PVP-gvOuId", "1234"),
                               new HttpHeader("X-PVP-ou", "egora"),
                               new HttpHeader("X-PVP-participantId", "test"),
                             };

        auth.PvpVersion = "2.1";
        auth.TimeToLive = 600;
      }
      else if (url == "http://egoratest/pvptestservice/1/")
      {
        auth = new CustomAuthorization();
        XmlDocument doc = new XmlDocument();
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
      else if (url == "http://egoratest/pvptestservice/2/")
      {
        PvpToken token = new PvpToken(PvpVersion.Version21);
        token.Attributes.Add(new PvpAttributeParticipantId("AT:L6:994"));
        token.Attributes.Add(new PvpAttributeUserId("fmeier@stmk.gv.at"));
        token.Attributes.Add(new PvpAttributePrincipalName("Meier"));
        token.Attributes.Add(new PvpAttributeGivenName("Franz"));
        token.Attributes.Add(new PvpAttributeMail("fmeier@stmk.gv.at"));
        token.Attributes.Add(new PvpAttributeGid("AT:B:0:UhO5RG++klaOTsVY+CU="));
        token.Attributes.Add(new PvpAttributeOuGvOuId("AT:L6:1299"));
        token.Attributes.Add(new PvpAttributeOu("L6AL-F2/c"));
        token.Attributes.Add(new PvpAttributeSecClass("2"));
        token.Attributes.Add(new PvpAttributeFunction("SB"));
        token.Attributes.Add(new PvpAttributeRoles("ZMR-Fremdenbehoerdenanfrage(GKZ=60100)"));

        auth = new CustomAuthorization();
        auth.SoapHeaderXmlFragment = token.GetSamlAttributeStatement();
        auth.PvpVersion = PvpVersionNumber.Version21;
        auth.TimeToLive = 600;
      }

      else if (url == "http://egoratest/pvptestapplication/saml/assertionconsumer.aspx")
      {
        PvpToken token = new PvpToken(PvpVersion.Version21);
        token.Attributes.Add(new PvpAttributeParticipantId("AT:L6:994"));
        token.Attributes.Add(new PvpAttributeUserId("fmeier@stmk.gv.at"));
        token.Attributes.Add(new PvpAttributePrincipalName("Meier"));
        token.Attributes.Add(new PvpAttributeGivenName("Franz"));
        token.Attributes.Add(new PvpAttributeMail("fmeier@stmk.gv.at"));
        token.Attributes.Add(new PvpAttributeGid("AT:B:0:UhO5RG++klaOTsVY+CU="));
        token.Attributes.Add(new PvpAttributeOuGvOuId("AT:L6:1299"));
        token.Attributes.Add(new PvpAttributeOu("L6AL-F2/c"));
        token.Attributes.Add(new PvpAttributeSecClass("2"));
        token.Attributes.Add(new PvpAttributeFunction("SB"));
        token.Attributes.Add(new PvpAttributeRoles("ZMR-Fremdenbehoerdenanfrage(GKZ=60100)"));

        auth = new CustomAuthorization();
        auth.SoapHeaderXmlFragment = token.GetSamlAttributeStatement();
        auth.PvpVersion = PvpVersionNumber.Version21;
        auth.TimeToLive = 0;
      }

      else if (url == "http://egoratest/pvpintegrated/" || url == "https://w0507.int.rubicon-it.com:9443/pvptestapplication/1/")
      {
        auth = new CustomAuthorization();

        auth.HttpHeaders = new HttpHeader[]
                             {
                               new HttpHeader("X-Version", "1.9"),
                               new HttpHeader("X-AUTHENTICATE-UserId", "niemand")
                               //userId.Split ('\\')[1]) //
                             };

        auth.TimeToLive = 600;
      }

      return auth;
    }

    #endregion
  }
}