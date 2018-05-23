/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;

namespace Egora.Stammportal.HttpReverseProxy.Administration
{
  public partial class Authorization : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      AuthorizationGridView.DataSource = AuthorizationWebServiceProxy.History;
      AuthorizationGridView.DataBind();
    }
  }
}