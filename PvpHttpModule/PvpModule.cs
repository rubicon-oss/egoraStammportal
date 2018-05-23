/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Threading;
using System.Security.Principal;

namespace Egora.PvpHttpModule
{
  public class PvpModule : IHttpModule
  {
    /// <summary>
    ///                     Initializes a module and prepares it to handle requests.
    /// </summary>
    /// <param name="context">
    ///                     An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application 
    ///                 </param>
    public virtual void Init(HttpApplication context)
    {
      context.AuthorizeRequest += Context_AuthorizeRequest;
    }

    /// <summary>
    ///                     Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
    /// </summary>
    public virtual void Dispose()
    {
      
    }

    public virtual void Context_AuthorizeRequest(object sender, EventArgs e)
    {
      HttpApplication application = (HttpApplication) sender;
      if (application.Context != null && application.Context.Request != null)
      {
        IPrincipal principal = new PvpPrincipal(application.Context.Request.Headers);
        application.Context.User = principal;
      }
    }

  }
}