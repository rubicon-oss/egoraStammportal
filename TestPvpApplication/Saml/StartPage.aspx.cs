using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Assertions;
using ComponentSpace.SAML2.Configuration;
using ComponentSpace.SAML2.Exceptions;

namespace TestPvpApplication.Saml
{
  public partial class StartPage : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request.QueryString["autoinitiate"] == "true")
        InitiateSamlSso();
    }

    private void InitiateSamlSso()
    {
      if (SAMLController.Configuration == null)
        SAMLController.Initialize(); 

      if (SAMLController.Configuration != null
          && SAMLController.Configuration.PartnerIdentityProviderConfigurations != null
          && SAMLController.Configuration.PartnerIdentityProviderConfigurations.Count > 0)
      {
        string returnUrl = Request.Url.ToString();
        SAMLServiceProvider.InitiateSSO(Response, returnUrl, SAMLController.Configuration.PartnerIdentityProviderConfigurations.First().Name);
        Response.End();
      }
    }

    protected void SPLogin_Click(object sender, EventArgs e)
    {
      InitiateSamlSso();
    }

  }
}