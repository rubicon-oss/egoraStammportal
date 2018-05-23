using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Egora.Pvp;

namespace TestPvpApplication
{
  public partial class PvpModuleTestPage : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      Dictionary<string, string> headers = new Dictionary<string, string>();
      var token = new PvpToken(Request.Headers);

      foreach (PvpAttributes h in Enum.GetValues(typeof (PvpAttributes)))
      {
        var val = token.GetAttributeValue(h);
        if (val != null)
          headers.Add(h.ToString(), val);
      }

      HeaderGrid.DataSource = headers;
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
      DataBind();
    }
  }
}
