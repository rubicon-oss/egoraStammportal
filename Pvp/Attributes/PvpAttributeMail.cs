using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeMail : PvpAttribute
  {
    public PvpAttributeMail()
      : base(

        friendlyName: "MAIL"
        , index: PvpAttributes.MAIL
        , samlAttributeName: "urn:oid:0.9.2342.19200300.100.1.3"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHENTICATE-mail"}
                        ,{PvpVersion.Version19, "X-AUTHENTICATE-mail"}
                        ,{PvpVersion.Version20, "X-PVP-MAIL"}
                        ,{PvpVersion.Version21, "X-PVP-MAIL"}
                      }
      , soapElementName: "mail"
        )
    {}

    public PvpAttributeMail(string value)
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
