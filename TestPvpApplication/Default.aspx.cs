/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;

namespace TestPvpApplication
{
  public partial class _Default : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      string userId = Request.Headers["X-AUTHENTICATE-UserId"];
      if (userId != null)
      {
        MessageLabel.Text = "Willkommen, " + userId;
      }
      else
      {
        MessageLabel.Text = "Sie haben keine Berechtigung.";
      }
    }
  }
}