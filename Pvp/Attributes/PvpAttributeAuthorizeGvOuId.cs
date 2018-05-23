using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeAuthorizeGvOuId : PvpAttribute
  {
    public PvpAttributeAuthorizeGvOuId()
      : base(

        friendlyName: "AUTHORIZE-GVOUID"
        , index: PvpAttributes.X_AUTHORIZE_gvOuId
        , samlAttributeName: null
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHORIZE-gvOuId"}
                        ,{PvpVersion.Version19, "X-AUTHORIZE-gvOuId"}
                      }
      , soapElementName: "gvOuId"
        )
    {}

    public PvpAttributeAuthorizeGvOuId(string value)
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
