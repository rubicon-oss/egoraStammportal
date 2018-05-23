/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using Egora.Stammportal.HttpReverseProxy.Mapping;

namespace Egora.Stammportal.HttpReverseProxy.Administration
{
  public partial class Applications : System.Web.UI.Page
  {
    protected RemoteApplication[] apps;

    protected void Page_Load(object sender, EventArgs e)
    {
      apps = RemoteApplication.GetApplications();
      ApplicationsGridView.DataSource = apps;
      ApplicationsGridView.DataBind();
    }

    protected void ApplicationsGridView_SelectedIndexChanged(object sender, EventArgs e)
    {
      InfoDetailsView.DataSource = new ApplicationDirectory[] {apps[ApplicationsGridView.SelectedIndex].Directory};
      InfoDetailsView.DataBind();
      HistoryGridView.DataSource = apps[ApplicationsGridView.SelectedIndex].History;
      HistoryGridView.DataBind();
    }
  }
}