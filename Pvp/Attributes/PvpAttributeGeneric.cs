using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeGeneric : PvpAttribute
  {
    private static List<string> s_identifiers = new List<string>();
 
    public static PvpAttributes GetGenericIndex(string identifier)
    {
      if (!s_identifiers.Contains(identifier))
        s_identifiers.Add(identifier);

      return (PvpAttributes) (int) PvpAttributes.Generic + s_identifiers.IndexOf(identifier);
    }

    public PvpAttributeGeneric(string friendlyName, string samlAttributeName, PvpVersion[] availableInVersions, Dictionary<PvpVersion, string> headerNames, string soapElementName)
      : base(

        friendlyName: friendlyName
        , index: GetGenericIndex(samlAttributeName ?? headerNames[availableInVersions[0]])
        , samlAttributeName: samlAttributeName
        , availableInVersions: availableInVersions
        , headerNames: headerNames
        , soapElementName: soapElementName
        )
    {}

  }
}
