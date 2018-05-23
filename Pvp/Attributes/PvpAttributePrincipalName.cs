using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributePrincipalName : PvpAttribute
  {
    public PvpAttributePrincipalName()
      : base(

        friendlyName: "PRINCIPAL-NAME"
        , index: PvpAttributes.PRINCIPAL_NAME
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.261.20"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version20, "X-PVP-PRINCIPAL-NAME"}
                        ,{PvpVersion.Version21, "X-PVP-PRINCIPAL-NAME"}
                      }
      , soapElementName: null
        )
    {}

    public PvpAttributePrincipalName(string value)
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
