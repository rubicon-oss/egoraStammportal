/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Egora.Stammportal.LdapAuthorizationService
{
  public class PvpAuthorization
  {
    public static PvpAuthorization Parse(string authorizationString)
    {
      string[] roleStrings = authorizationString.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      List<PvpRole> roles = new List<PvpRole>();
      foreach (string roleString in roleStrings)
        roles.Add(PvpRole.Parse(roleString));

      PvpAuthorization auth = new PvpAuthorization();
      auth._roles = roles;

      return auth;
    }

    private List<PvpRole> _roles = new List<PvpRole>();

    public List<PvpRole> Roles
    {
      get { return _roles; }
      set { _roles = value; }
    }

    public void Merge(string authorizationString)
    {
      Merge(Parse(authorizationString));
    }

    public void Merge(PvpAuthorization authorization)
    {
      foreach (PvpRole role in authorization.Roles)
        Merge(role);
    }

    public void Merge(PvpRole role)
    {
      PvpRole existingRole = GetRole(role.Name);
      if (existingRole == null)
      {
        Roles.Add(role);
      }
      else
      {
        existingRole.Merge(role.Parameters);
      }
    }

    public PvpRole GetRole(string name)
    {
      foreach (PvpRole role in Roles)
      {
        if (role.Name == name)
          return role;
      }
      return null;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      foreach (PvpRole role in Roles)
      {
        string roleString = role.ToString();
        if (!String.IsNullOrEmpty(roleString))
          sb.Append(roleString + ";");
      }

      return sb.ToString();
    }

    public string GetSoapFragment()
    {
      StringBuilder sb = new StringBuilder();
      foreach (PvpRole role in Roles)
        sb.Append(role.GetSoapFragment());

      return sb.ToString();
    }
  }
}