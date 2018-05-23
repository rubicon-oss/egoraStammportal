/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

namespace Egora.Stammportal
{
  /// <summary>
  /// The Reverse Proxy uses this interface.
  /// An implementation of this interface needs access to user specific data.
  /// </summary>
  public interface IAuthorizationContract
  {
    /// <summary>
    /// Get the authorization for the given application, user and context.
    /// </summary>
    /// <param name="rootUrl">The root URL of the Application</param>
    /// <param name="userId">The UserId of the user initiating the request. Depends on the authentication method.</param>
    /// <returns>The Authorization for given application and user.</returns>
    CustomAuthorization GetAuthorization(string rootUrl, string userId);
  }
}