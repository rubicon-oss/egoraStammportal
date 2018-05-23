using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeSecClass : PvpAttribute
  {
    public PvpAttributeSecClass()
      : base(
        friendlyName: "SECCLASS"
        , index: PvpAttributes.SECCLASS
        , samlAttributeName: "http://lfrz.at/stdportal/names/pvp/secClass"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-AUTHENTICATE-gvSecClass"}
                        ,{PvpVersion.Version19, "X-AUTHENTICATE-gvSecClass"}
                        ,{PvpVersion.Version20, "X-PVP-SECCLASS"}
                        ,{PvpVersion.Version21, "X-PVP-SECCLASS"}
                      }
      , soapElementName: "gvSecClass"
        )
    { }

    public PvpAttributeSecClass(string value)
      : this()
    {
      Value = value;
    }

    public string[] AllowedValues = new string[]{"1", "2", "3"};

    public override void CheckValue(string value)
    {
      if (!AllowedValues.Contains(value))
        throw new PvpException("SECCLASS Wert " + value + " wird nicht unterstützt.");
    }
  }
}
