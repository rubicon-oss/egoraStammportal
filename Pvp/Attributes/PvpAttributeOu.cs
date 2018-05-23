using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeOu : PvpAttribute
  {
    public PvpAttributeOu()
      : base(

        friendlyName: "OU"
        , index: PvpAttributes.OU
        , samlAttributeName: "urn:oid:2.5.4.11"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHENTICATE-Ou"}
                        ,{PvpVersion.Version19, "X-AUTHENTICATE-Ou"}
                        ,{PvpVersion.Version20, "X-PVP-OU"}
                        ,{PvpVersion.Version21, "X-PVP-OU"}
                      }
      , soapElementName: "ou"
        )
    {}

    public PvpAttributeOu(string value)
      : this()
    {
      Value = value;
    }

    public override void CheckValue(string value)
    {
      StringMaxLenCheck(value, 64);
    }
  }
}
