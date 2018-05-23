/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;
using System.Web;

namespace TestPvpApplication
{
  public partial class TestPage : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      UserLabel.Text = User.Identity.Name;
      AuthenticatedCheckBox.Checked = User.Identity.IsAuthenticated;
    }

    protected void SetCookieButton_Click(object sender, EventArgs e)
    {
      HttpCookie cookie = new HttpCookie(NameField.Text, ValueField.Text);
      DomainLabel.Text = cookie.Domain;
      PathLabel.Text = cookie.Path;
      Response.SetCookie(cookie);
    }

    protected void RedirectButton_Click(object sender, EventArgs e)
    {
      Response.Redirect("TestPage.aspx");
    }
  }
}