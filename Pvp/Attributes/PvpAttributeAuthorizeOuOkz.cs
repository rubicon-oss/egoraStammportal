using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeAuthorizeOuOkz : PvpAttribute
  {
    public PvpAttributeAuthorizeOuOkz()
      : base(

        friendlyName: "AUTHORIZE-GVOUOKZ"
        , index: PvpAttributes.X_AUTHORIZE_GvOuOkz
        , samlAttributeName: null
        , availableInVersions: new PvpVersion[] { PvpVersion.Version19 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version19, "X-AUTHORIZE-gvOuOKZ"}
                      }
      , soapElementName: "gvOuOKZ"
        )
    {}

    public PvpAttributeAuthorizeOuOkz(string value)
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
