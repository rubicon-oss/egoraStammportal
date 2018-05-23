using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeTel : PvpAttribute
  {
    public PvpAttributeTel()
      : base(

        friendlyName: "TEL"
        , index: PvpAttributes.TEL
        , samlAttributeName: "urn:oid:2.5.4.20"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHENTICATE-tel"}
                        ,{PvpVersion.Version19, "X-AUTHENTICATE-tel"}
                        ,{PvpVersion.Version20, "X-PVP-TEL"}
                        ,{PvpVersion.Version21, "X-PVP-TEL"}
                      }
      , soapElementName: "tel"
        )
    {}

    public PvpAttributeTel(string value)
      : this()
    {
      Value = value;
    }

    public override void CheckValue(string value)
    {
      StringMaxLenCheck(value, 32);
    }
  }
}
