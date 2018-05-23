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
public class HeadertestAuthorizer : WebService, IAuthorizationContract
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
    rootUrl = rootUrl.ToLowerInvariant();

    CustomAuthorization auth = new CustomAuthorization();
    auth.TimeToLive = 1;

    if (rootUrl.EndsWith("headertestpage.aspx"))
    {
      List<HttpHeader> headers = new List<HttpHeader>();
      AddIfNotEmpty(headers, "X-Version", "1.9");
      AddIfNotEmpty(headers, "X-AUTHENTICATE-UserId", userId.Replace(@"\", "_"));
      auth.HttpHeaders = headers.ToArray();
    }

    else
    {
      auth = CustomAuthorization.NoAuthorization;
    }

    return auth;
  }

  private void AddIfNotEmpty(List<HttpHeader> headers, string headerName, string headerValue)
  {
    if (!String.IsNullOrEmpty(headerValue))
      headers.Add(new HttpHeader(headerName, headerValue));
  }
}