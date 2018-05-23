using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeVersion : PvpAttribute
  {
    public PvpAttributeVersion() : base(
      friendlyName : "PVP-VERSION",
      index : PvpAttributes.VERSION,
      samlAttributeName : "urn:oid:1.2.40.0.10.2.1.1.261.10",
      availableInVersions : new PvpVersion[]{PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21},
      headerNames : new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-Version"}
                        ,{PvpVersion.Version19, "X-Version"}
                        ,{PvpVersion.Version20, "X-PVP-EGOVTOKEN-VERSION"}
                        ,{PvpVersion.Version21, "X-PVP-VERSION"}
                      }
      , soapElementName: null
      )
    {}

    public PvpAttributeVersion(string value)
      : this()
    {
      Value = value; 
    }

    public override void CheckValue(string value)
    {
      if (!PvpVersionNumber.PvpVersions.ContainsKey(value))
        throw new PvpException("Version " + value + " wird nicht unterstützt.");
    }

    public override PvpVersion CurrentVersion
    {
      get
      {
        return base.CurrentVersion;
      }
      internal set
      {
        base.CurrentVersion = value;
        Value = PvpVersionNumber.PvpVersions.Single(p => p.Value == value).Key;
      }
    }
  }
}
