/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System.Collections.Specialized;
using System.Security.Principal;
using Egora.Pvp;

namespace Egora.PvpHttpModule
{
  public class PvpPrincipal : IPvpPrincipal
  {
    private PvpToken _token;


    public PvpPrincipal(NameValueCollection headers)
    {
      if (PvpToken.DeterminePvpVersion(headers).HasValue)
        _token = new PvpToken(headers);
    }

    /// <summary>
    ///                     Determines whether the current principal belongs to the specified role.
    /// </summary>
    /// <returns>
    /// true if the current principal is a member of the specified role; otherwise, false.
    /// </returns>
    /// <param name="roleName">
    ///                     The name of the role for which to check membership. 
    ///                 </param>
    public bool IsInRole(string roleName)
    {
      return _token != null && _token.RoleAttribute != null && _token.RoleAttribute.HasRole(PvpRole.Parse(roleName));
    }

    public bool IsInRole(string roleName, NameValueCollection roleParameters)
    {
      return _token != null && _token.RoleAttribute != null && _token.RoleAttribute.HasRole(new PvpRole(roleName, roleParameters));
    }

    /// <summary>
    ///                     Gets the identity of the current principal.
    /// </summary>
    /// <returns>
    ///                     The <see cref="T:System.Security.Principal.IIdentity" /> object associated with the current principal.
    /// </returns>
    public IIdentity Identity
    {
      get { return _token; }
    }

    public string GvOuId
    {
      get { return (_token == null) ? null : _token.GetAttributeValue(PvpAttributes.OU_GV_OU_ID); }
      set { throw new System.NotImplementedException(); }
    }

    public string Ou
    {
      get { return (_token == null) ? null : _token.GetAttributeValue(PvpAttributes.OU); }
      set { throw new System.NotImplementedException(); }
    }

    public IPvpRole[] GetRoles()
    {
      return (_token == null || _token.RoleAttribute == null) ? null : _token.GetRoles();
    }

    public PvpToken GetToken()
    {
      return _token;
    }

    public IPvpRole GetRole(string name)
    {
      return (_token == null || _token.RoleAttribute == null) ? null : _token.RoleAttribute.GetRole(name);
    }
  }
}