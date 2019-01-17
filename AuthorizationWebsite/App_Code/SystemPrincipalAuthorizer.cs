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
public class SystemPrincipalAuthorizer : System.Web.Services.WebService
{

  public SystemPrincipalAuthorizer()
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
    if (rootUrl.Equals("dummy", StringComparison.InvariantCultureIgnoreCase))
    {
      var version = "1.9";
      var pvpToken = new PvpToken(new Dictionary<PvpAttributes, string>()
                                  {
                                    {PvpAttributes.VERSION, version},
                                    {PvpAttributes.PARTICIPANT_ID, "AT:L6:1234789"},
                                    {PvpAttributes.USERID, "egovstar.appserv1.intra.xyz.gv.at"},
                                    {PvpAttributes.X_AUTHENTICATE_cn, "Anwendung 1 Register-Interface"},
                                    {PvpAttributes.OU_GV_OU_ID, "AT:L6:4711"},
                                    {PvpAttributes.OU, "Fachabteilung 1B Informationstechnik"},
                                    {PvpAttributes.SECCLASS, "2"},
                                    {PvpAttributes.ROLES, "FixedRole(param=value)"},
                                  }, false);

      CustomAuthorization auth = new CustomAuthorization();
      auth.TimeToLive = 60 * 10; //10 Minuten
      auth.PvpVersion = version;
      auth.SoapHeaderXmlFragment = pvpToken.GetSystemPrincipalSoapFragment();

      return auth;
    }

    return CustomAuthorization.NoAuthorization;
  }

}
/*
<authenticate>
  <participantId>AT:L6:1234789</participantId>
  <systemPrincipal>
    <userId>egovstar.appserv1.intra.xyz.gv.at</userId>
    <cn>Anwendung 1 Register-Interface</cn>
    <gvOuId>AT:L6:4711</gvOuId>
    <ou>Fachabteilung 1B Informationstechnik</ou>
    <gvSecClass>2</gvSecClass>
  </systemPrincipal>
</authenticate>
<authorize>
  <role value="FixedRole">
    <param>
      <key>param</key>
      <value>value</value>
    </param>
  </role>
</authorize></authorize> 
*/
