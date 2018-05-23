using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public sealed class PvpAttributeRoles : PvpAttribute
  {
    public PvpAttributeRoles()
      : base(

        friendlyName: "ROLES"
        , index: PvpAttributes.ROLES
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.261.30"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHORIZE-roles"}
                        ,{PvpVersion.Version19, "X-AUTHORIZE-roles"}
                        ,{PvpVersion.Version20, "X-PVP-ROLES"}
                        ,{PvpVersion.Version21, "X-PVP-ROLES"}
                      }
      , soapElementName: null
        )
    {}

    public PvpAttributeRoles(string value)
      : this()
    {
      Value = value;
    }

    public PvpAttributeRoles(string[] values)
      : this()
    {
      SetValues(values);
    }

    private List<PvpRole> _roles = new List<PvpRole>();

    public List<PvpRole> Roles
    {
      get { return _roles; }
      set { _roles = value; }
    }
    
    public override string Value
    {
      get { return String.Join(";", Roles.Select(r => r.ToString())); }
      set
      {
        SetValues(new string[]{value});
      }
    }

    private string[] GetRoleStrings(string rolesStrings)
    {
      return rolesStrings.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    }

    public override void SetValues(string[] values)
    {
      Roles.Clear();
      foreach (string r in values)
      {
        Merge(r);
      }
    }

    public void Merge(string rolesString)
    {
      foreach (string roleString in GetRoleStrings(rolesString))
      {
        Merge(PvpRole.Parse(roleString, true));
      }
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
        if (role.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
          return role;
      }
      return null;
    }
    
    public override void CheckValue(string value)
    {
      StringMaxLenCheck(value, 32767);
    }

    public override string GetXmlPart()
    {
      return String.Join(Environment.NewLine, Roles.Select(r => r.GetSoapFragment()));
    }

    public bool HasRole(PvpRole otherRole)
    {
      PvpRole role = GetRole(otherRole.Name);
      if (role == null)
        return false;

      return role.Contains(otherRole);
    }
  }
}
