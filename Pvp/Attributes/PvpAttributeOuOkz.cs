using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeOuOkz : PvpAttribute
  {
    public PvpAttributeOuOkz()
      : base(

        friendlyName: "OU-OKZ"
        , index: PvpAttributes.OU_OKZ
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.153"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version19, "X-AUTHENTICATE-gvOuOKZ"}
                        ,{PvpVersion.Version20, "X-PVP-OU-OKZ"}
                        ,{PvpVersion.Version21, "X-PVP-OU-OKZ"}
                      }
      , soapElementName: "gvOuOKZ"
        )
    {}

    public PvpAttributeOuOkz(string value)
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
