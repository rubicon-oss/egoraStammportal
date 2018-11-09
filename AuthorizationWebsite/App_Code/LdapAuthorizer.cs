/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;
using System.Collections.Generic;
using System.Web.Services;
using Egora.Pvp;
using Egora.Stammportal;
using Egora.Stammportal.LdapAuthorizationService;

[WebService(Namespace = CustomAuthorization.Namespace)]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class LdapAuthorizer : WebService, IAuthorizationContract
{
  /// <summary>
  /// Gets the authorization information for a given application and user
  /// </summary>
  /// <param name="rootUrl">RootUrl of Application</param>
  /// <param name="userId">Unique identifier of a user, e.g. loginname</param>
  /// <returns>HttpHeaders for ApplicationType Web, SOAP Header for ApplcationType SOAP</returns>
  [WebMethod]
  public CustomAuthorization GetAuthorization(string rootUrl, string userId)
  {
    LdapConfiguration configuration = LdapConfiguration.GetConfiguration();

    rootUrl = rootUrl.ToLowerInvariant();
    PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer(rootUrl, userId, configuration);

    if (!authorizer.IsValid)
      return null;

    CustomAuthorization auth = new CustomAuthorization();
    auth.TimeToLive = authorizer.AuthorizationTimeToLive;
    auth.PvpVersion = authorizer.Version;

    if (authorizer.IsWeb)
    {
      if (rootUrl.Contains("assertion"))
      {
        auth.SoapHeaderXmlFragment = authorizer.GetPvpToken().GetSamlAttributeStatement();
      }
      else
      {
        List<HttpHeader> headers = authorizer.GetPvpToken().GetHeaders();
        auth.HttpHeaders = headers.ToArray();
      }
    }
    else if (authorizer.IsSoap)
    {
      auth.SoapHeaderXmlFragment = authorizer.UserPrincipalSoapFragment;
    }
    else
    {
      auth = CustomAuthorization.NoAuthorization;
    }

    return auth;
  }

}