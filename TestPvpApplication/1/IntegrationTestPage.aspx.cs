/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;
using System.Web;
using System.Xml.Serialization;
using Egora.Stammportal.HttpReverseProxy.UnitTests.IntegrationTest;

namespace TestPvpApplication
{
  public partial class IntegrationTestPage : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request.QueryString["CookieName"] != null)
      {
        string name = Request.QueryString["CookieName"];
        string value = Request.QueryString["CookieValue"];
        HttpCookie cookie = new HttpCookie(name, value);
        cookie.Path = Page.ResolveUrl(Request.QueryString["CookiePath"]);
        bool httpOnly = String.IsNullOrEmpty(Request.QueryString["HttpOnly"])
                          ? false
                          : Boolean.Parse(Request.QueryString["HttpOnly"]);
        cookie.HttpOnly = httpOnly;
        Response.SetCookie(cookie);
      }

      XmlSerializer serializer = new XmlSerializer(typeof (RequestInformation));
      RequestInformation info = new RequestInformation(Request);
      serializer.Serialize(Response.OutputStream, info);
    }
  }
}