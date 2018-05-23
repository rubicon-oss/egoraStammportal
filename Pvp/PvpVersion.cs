using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp
{
  /*
   *
Wert  Bedeutung
1.0   PVP 1.4 (BMI Gateway Protokoll)
1.1   PVP 1.5.3
1.2   PVP 1.6, PVP 1.7
1.8   PVP 1.8.x
1.9   PVP 1.9.x
2.0   PVP 2.0.x
2.1   PVP 2.1.x

Versionen vor 1.8 werden nicht implementiert
   */
  public class PvpVersionNumber
  {
    public const string Version18 = "1.8";
    public const string Version19 = "1.9";
    public const string Version20 = "2.0";
    public const string Version21 = "2.1";

    public static Dictionary<string, PvpVersion> PvpVersions = new Dictionary<string, PvpVersion>()
                                                                 {
                                                                    {Version18, PvpVersion.Version18}
                                                                   ,{Version19, PvpVersion.Version19}
                                                                   ,{Version20, PvpVersion.Version20}
                                                                   ,{Version21, PvpVersion.Version21}
                                                                 };
  }

  public enum PvpVersion
  {
    Version18 = 18,
    Version19 = 19,
    Version20 = 20,
    Version21 = 21
  }

 
}
