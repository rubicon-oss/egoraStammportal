using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.SessionState;
using Egora.Stammportal.Authentication;

namespace Egora.StammportalForms
{
  public class Global : System.Web.HttpApplication
  {

    public const string AuthCookieName = "WinAuth";
    protected void Application_Start(object sender, EventArgs e)
    {

    }

    protected void Session_Start(object sender, EventArgs e)
    {

    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {

    }

    //This method is called just before authentication is performed. This is time and place where we can write our own authentication codes.
    void Application_AuthenticateRequest(object sender, EventArgs e)
    {
      var authInfo = AuthenticationInformation.LoginToFormsAuthentication();
      if (authInfo != null)
      {
        Response.Redirect(Request.RawUrl);
      }
    }

    protected void Application_Error(object sender, EventArgs e)
    {

    }

    protected void Session_End(object sender, EventArgs e)
    {

    }

    protected void Application_End(object sender, EventArgs e)
    {

    }
  }
}