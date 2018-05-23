using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeParticipantOkz : PvpAttribute
  {
    public PvpAttributeParticipantOkz()
      : base(

        friendlyName: "PARTICIPANT-OKZ"
        , index: PvpAttributes.PARTICIPANT_OKZ
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.261.24"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                        {PvpVersion.Version21, "X-PVP-PARTICIPANT-OKZ"}
                      }
      , soapElementName: null
        )
    {}

    public PvpAttributeParticipantOkz(string value)
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
