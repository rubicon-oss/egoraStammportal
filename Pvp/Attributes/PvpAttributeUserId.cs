using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeUserId : PvpAttribute
  {
    public PvpAttributeUserId()
      : base(

        friendlyName: "USERID"
        , index: PvpAttributes.USERID
        , samlAttributeName: "urn:oid:0.9.2342.19200300.100.1.1"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHENTICATE-UserID"}
                        ,{PvpVersion.Version19, "X-AUTHENTICATE-UserID"}
                        ,{PvpVersion.Version20, "X-PVP-USERID"}
                        ,{PvpVersion.Version21, "X-PVP-USERID"}
                      }
      , soapElementName: "userId"  
      )
    {}

    public PvpAttributeUserId(string value)
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
