using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeChargeCode : PvpAttribute
  {
    public PvpAttributeChargeCode()
      : base(

        friendlyName: "CHARGE-CODE"
        , index: PvpAttributes.CHARGE_CODE
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.261.60"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-ACCOUNTING-ChargeCode"}
                        ,{PvpVersion.Version19, "X-ACCOUNTING-ChargeCode"}
                        ,{PvpVersion.Version20, "X-PVP-CHARGE-CODE"}
                        ,{PvpVersion.Version21, "X-PVP-CHARGE-CODE"}
                      }
      , soapElementName: "ChargeCode"
        )
    {}

    public PvpAttributeChargeCode(string value)
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
