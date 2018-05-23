using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeEncBpkList : PvpAttribute
  {
    public PvpAttributeEncBpkList()
      : base(

        friendlyName: "ENC-BPK-LIST"
        , index: PvpAttributes.ENC_BPK_LIST
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.261.22"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version20, "X-PVP-ENC-BPK-LIST"}
                        ,{PvpVersion.Version21, "X-PVP-ENC-BPK-LIST"}
                      }
      , soapElementName: null
        )
    {}

    public PvpAttributeEncBpkList(string value)
      : this()
    {
      Value = value;
    }

    public override void CheckValue(string value)
    {
      StringMaxLenCheck(value, 32767);
    }
  }
}
