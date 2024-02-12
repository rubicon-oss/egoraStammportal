using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egora.Stammportal.Authentication
{


  public class Settings : System.Configuration.ApplicationSettingsBase
  {

    private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

    public static Settings Default
    {
      get
      {
        return defaultInstance;
      }
    }

    [global::System.Configuration.ApplicationScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("StammportalLogin")]
    public string LoginCookieName
    {
      get
      {
        return ((string)(this["LoginCookieName"]));
      }
    }
  }
}

