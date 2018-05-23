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
  public partial class Error500 : System.Web.UI.Page
  {
    private static int s_counter;

    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request.QueryString["reset"] != null)
        s_counter = 0;

      s_counter++;
      if ((s_counter%2) == 0)
      {
        Response.StatusCode = 500;
        Response.StatusDescription = "Testexception";
        Response.End();
      }
    }
  }
}