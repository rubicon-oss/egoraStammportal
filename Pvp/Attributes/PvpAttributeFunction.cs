using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeFunction : PvpAttribute
  {
    public PvpAttributeFunction()
      : base(

        friendlyName: "FUNCTION"
        , index: PvpAttributes.FUNCTION
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.33"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHENTICATE-gvFunction"}
                        ,{PvpVersion.Version19, "X-AUTHENTICATE-gvFunction"}
                        ,{PvpVersion.Version20, "X-PVP-FUNCTION"}
                        ,{PvpVersion.Version21, "X-PVP-FUNCTION"}
                      }
      , soapElementName: "gvFunction"
        )
    {}

    public PvpAttributeFunction(string value)
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
