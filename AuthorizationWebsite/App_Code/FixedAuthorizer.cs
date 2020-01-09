using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using Egora.Pvp;
using Egora.Stammportal;
using Egora.Stammportal.LdapAuthorizationService;

/// <summary>
/// Summary description for FixedAuthorizer
/// </summary>
[WebService(Namespace = CustomAuthorization.Namespace)]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class FixedAuthorizer : System.Web.Services.WebService
{

  public FixedAuthorizer()
  {

    //Uncomment the following line if using designed components 
    //InitializeComponent(); 
  }

  /// <summary>
  /// Gets the authorization information for a given application and user
  /// </summary>
  /// <param name="rootUrl">RootUrl of Application</param>
  /// <param name="userId">Unique identifier of a user, e.g. loginname</param>
  /// <returns>HttpHeaders for ApplicationType Web, SOAP Header for ApplicationType SOAP</returns>
  [WebMethod]
  public CustomAuthorization GetAuthorization(string rootUrl, string userId)
  {
    var file = Path.Combine(Server.MapPath("~"), "ConfigurationFixed.xml");
    LdapConfiguration configuration = LdapConfiguration.GetConfiguration(file);

    rootUrl = rootUrl.ToLowerInvariant();
    PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer(rootUrl, userId, configuration);

    if (!authorizer.IsValid)
      return  CustomAuthorization.NoAuthorization;

    CustomAuthorization auth = new CustomAuthorization();
    auth.TimeToLive = authorizer.AuthorizationTimeToLive;
    auth.PvpVersion = authorizer.Version;
    var dummy = authorizer.GvGID;
    var chainedToken = authorizer.GetPvpToken().GetChainedSoapFragment();
    var token = String.Format(
@"<pvpToken version=""{0}"" xmlns=""http://egov.gv.at/pvp1.xsd"">
<authenticate>
  <participantId>{1}</participantId>
  <systemPrincipal>
    <userId>egovstar.appserv1.intra.xyz.gv.at</userId>
    <cn>Anwendung 1 Register-Interface</cn>
    <gvOuId>AT:L6:4711</gvOuId>
    <ou>Fachabteilung 1B Informationstechnik</ou>
    <gvOuID>{2}</gvOuID>
    <gvSecClass>{3}</gvSecClass>
  </systemPrincipal>
</authenticate>
<authorize>
  <role value=""Registerabfrage""></role>
</authorize>
{4}
</pvpToken>",
      authorizer.Version,
      authorizer.ParticipantID,
      authorizer.GvOuID,
      authorizer.GvSecClass,
      chainedToken.OuterXml);
     
    XmlDocument doc = new XmlDocument();
    doc.LoadXml(token);
    auth.SoapHeaderXmlFragment = doc.DocumentElement;

    return auth;
  }

}
/*
<pvpChainedToken version = ""1.9"">
<authenticate>
<participantId>AT:L6:1234789</participantId>
<userPrincipal>
<userId>mmustermann @kommunalnet.at</userId>
<cn>Max Mustermann</cn>
<gvOuId>AT:GGA-60420:0815</gvOuId>
<ou>Meldeamt</ou>
<gvOuOKZ>AT:GGA-60420-Abt13</gvOuOKZ>
<gvSecClass>2</gvSecClass>
<gvGid>AT:B:0:LxXnvpcYZesiqVXsZG0bB==</gvGid>
<mail>max.mustermann @hatzendorf.steiermark.at</mail>
<tel>+43 3155 5153</tel>
</userPrincipal>
</authenticate>
<authorize>
<role value = ""Beispielrolle""> <param> <key>GKZ</key> <value>60420</value> </param> </role>
</authorize>
</pvpChainedToken>
*/