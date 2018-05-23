/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Net;

namespace Egora.Stammportal.HttpReverseProxy.Administration
{
  public partial class Connection : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      List<ServicePoint> servicePoints = new List<ServicePoint>();
      foreach (RemoteApplication app in RemoteApplication.GetApplications())
      {
        ServicePoint servicePoint = ServicePointManager.FindServicePoint(new Uri(app.RootUrl));
        if (!servicePoints.Contains(servicePoint))
          servicePoints.Add(servicePoint);
      }

      ConnectionGridView.DataSource = servicePoints;
      ConnectionGridView.DataBind();
    }
  }
}