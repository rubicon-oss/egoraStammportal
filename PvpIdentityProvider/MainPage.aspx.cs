using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Configuration;
using ComponentSpace.SAML2.Exceptions;

namespace Egora.Stammportal.PvpIdentityProvider
{
  public partial class MainPage : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      if (SAMLController.Configuration == null)
        SAMLController.Initialize();

      if (SAMLController.Configuration != null
          && SAMLController.Configuration.PartnerServiceProviderConfigurations != null
          && SAMLController.Configuration.PartnerServiceProviderConfigurations.Count > 0)
      {
        ServiceProviderDropDown.Items.AddRange(
          SAMLController.Configuration.PartnerServiceProviderConfigurations.Select(c => new ListItem(c.Name, c.AssertionConsumerServiceUrl)).ToArray());
      }

    }

    protected void LoginButton_Click(object sender, EventArgs e)
    {
      string userName = PvpSamlLoginHandler.GetUserName(Request);
      var config = SAMLController.Configuration.PartnerServiceProviderConfigurations
        .First(sp => sp.AssertionConsumerServiceUrl.Equals(ServiceProviderDropDown.SelectedItem.Value));
      var attributes = PvpSamlLoginHandler.GetSamlAttributes(config, userName);
      SAMLIdentityProvider.InitiateSSO(Response, userName, attributes, null, ServiceProviderDropDown.SelectedItem.Value);
    }
  }
}