using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Egora.Stammportal.PvpIdentityProvider
{
  public static class SecClassHelper
  {
    public static int? SecClass
    {
      get => (int?)HttpContext.Current.Session["SecClass"];
      set => HttpContext.Current.Session["SecClass"] = value;
    }
  }
}