using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeBirthDate : PvpAttribute
  {
    public PvpAttributeBirthDate()
      : base(

        friendlyName: "BIRTHDATE"
        , index: PvpAttributes.BIRTHDATE
        , samlAttributeName: "urn:oid:2.5.4.42"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version20, "X-PVP-BIRTHDATE"}
                        ,{PvpVersion.Version21, "X-PVP-BIRTHDATE"}
                      }
      , soapElementName: null
        )
    {}

    public PvpAttributeBirthDate(string value)
      : this()
    {
      Value = value;
    }

    public override void CheckValue(string value)
    {
      if (value == null)
        return;
      string[] parts = value.Split('-');
      if (parts.Length!=3)
        throw new PvpException("Geburtsdatum " + value + " ungültig, es muss Format JJJJ-MM-TT haben.");
      if (parts[1] != "00" && parts[2] != "00")
      {
        DateTime date;
        if (!DateTime.TryParseExact(value, "yyyy-MM-dd", null, DateTimeStyles.None, out date))
          throw new PvpException(value + " ist kein gültiges Datum.");
      }
    }
  }
}
