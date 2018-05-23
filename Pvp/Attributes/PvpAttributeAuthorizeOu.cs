using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeAuthorizeOu : PvpAttribute
  {
    public PvpAttributeAuthorizeOu()
      : base(

        friendlyName: "AUTHORIZE-OU"
        , index: PvpAttributes.X_AUTHORIZE_Ou
        , samlAttributeName: null
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHORIZE-Ou"}
                        ,{PvpVersion.Version19, "X-AUTHORIZE-Ou"}
                      }
      , soapElementName: "ou"
        )
    {}

    public PvpAttributeAuthorizeOu(string value)
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
