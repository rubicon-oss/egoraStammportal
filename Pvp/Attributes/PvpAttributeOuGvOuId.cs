using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeOuGvOuId : PvpAttribute
  {
    public PvpAttributeOuGvOuId()
      : base(

        friendlyName: "OU-GV-OU-ID"
        , index: PvpAttributes.OU_GV_OU_ID
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.3"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHENTICATE-gvOuId"}
                        ,{PvpVersion.Version19, "X-AUTHENTICATE-gvOuId"}
                        ,{PvpVersion.Version20, "X-PVP-OU-GV-OU-ID"}
                        ,{PvpVersion.Version21, "X-PVP-OU-GV-OU-ID"}
                      }
      , soapElementName: "gvOuId"
        )
    {}

    public PvpAttributeOuGvOuId(string value)
      : this()
    {
      Value = value;
    }

    public override void CheckValue(string value)
    {
      StringMaxLenCheck(value, 39);
    }
  }
}
