using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeGid : PvpAttribute
  {
    public PvpAttributeGid()
      : base(

        friendlyName: "GID"
        , index: PvpAttributes.GID
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.1"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHENTICATE-gvGid"}
                        ,{PvpVersion.Version19, "X-AUTHENTICATE-gvGid"}
                        ,{PvpVersion.Version20, "X-PVP-GID"}
                        ,{PvpVersion.Version21, "X-PVP-GID"}
                      }
      , soapElementName: "gvGid"
        )
    {}

    public PvpAttributeGid(string value)
      : this()
    {
      Value = value;
    }

    public override void CheckValue(string value)
    {
      StringMaxLenCheck(value, 128);
    }
  }
}
