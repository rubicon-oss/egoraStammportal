using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeCostCenterId : PvpAttribute
  {
    public PvpAttributeCostCenterId()
      : base(

        friendlyName: "COST-CENTER-ID"
        , index: PvpAttributes.COST_CENTER_ID
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.261.50"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-ACCOUNTING-CostCenterId"}
                        ,{PvpVersion.Version19, "X-ACCOUNTING-CostCenterId"}
                        ,{PvpVersion.Version20, "X-PVP-COST-CENTER-ID"}
                        ,{PvpVersion.Version21, "X-PVP-COST-CENTER-ID"}
                      }
      , soapElementName: "CostCenterId"
        )
    {}

    public PvpAttributeCostCenterId(string value)
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
