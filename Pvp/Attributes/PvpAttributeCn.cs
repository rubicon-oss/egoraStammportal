using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeCn : PvpAttribute
  {
    public PvpAttributeCn()
      : base(

        friendlyName: "CN"
        , index: PvpAttributes.X_AUTHENTICATE_cn
        , samlAttributeName: null
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHENTICATE-cn"}
                        ,{PvpVersion.Version19, "X-AUTHENTICATE-cn"}
                      }
      , soapElementName: "cn"
        )
    {}

    public PvpAttributeCn(string value)
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
