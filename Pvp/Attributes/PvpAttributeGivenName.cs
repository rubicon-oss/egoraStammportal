using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeGivenName : PvpAttribute
  {
    public PvpAttributeGivenName()
      : base(

        friendlyName: "GIVEN-NAME"
        , index: PvpAttributes.GIVEN_NAME
        , samlAttributeName: "urn:oid:2.5.4.42"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version20, "X-PVP-GIVEN-NAME"}
                        ,{PvpVersion.Version21, "X-PVP-GIVEN-NAME"}
                      }
      , soapElementName: null
        )
    {}

    public PvpAttributeGivenName(string value)
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
