using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeBpk : PvpAttribute
  {
    public PvpAttributeBpk()
      : base(

        friendlyName: "BPK"
        , index: PvpAttributes.BPK
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.149"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version19, "X-AUTHENTICATE-gvBpk"}
                        ,{PvpVersion.Version20, "X-PVP-BPK"}
                        ,{PvpVersion.Version21, "X-PVP-BPK"}
                      }
      , soapElementName: "gvBpk"
        )
    {}

    public PvpAttributeBpk(string value)
      : this()
    {
      Value = value;
    }

    public override void CheckValue(string value)
    {
      StringMaxLenCheck(value, 1024);
    }
  }
}
