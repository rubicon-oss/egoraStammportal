using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeParticipantId : PvpAttribute
  {
    public PvpAttributeParticipantId()
      : base(

        friendlyName: "PARTICIPANT-ID"
        , index: PvpAttributes.PARTICIPANT_ID
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.71"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHENTICATE-participantId"}
                        ,{PvpVersion.Version19, "X-AUTHENTICATE-participantId"}
                        ,{PvpVersion.Version20, "X-PVP-PARTICIPANT-ID"}
                        ,{PvpVersion.Version21, "X-PVP-PARTICIPANT-ID"}
                      }
      , soapElementName: "participantId"
        )
    {}

    public PvpAttributeParticipantId(string value)
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
