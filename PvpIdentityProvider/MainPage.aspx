<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="ComponentSpace.SAML2" %>
<%@ Import Namespace="Egora.Stammportal.PvpIdentityProvider" %>

<!DOCTYPE html>
<script runat="server">

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
        var spName = ServiceProviderDropDown.SelectedItem.Value;
        var spConfig = SAMLController.Configuration.PartnerServiceProviderConfigurations
            .First(sp => sp.AssertionConsumerServiceUrl.Equals(spName));
        string pvpVersion;
        int secClass;
        string authnContext;
        var attributes = PvpSamlLoginHandler.GetSamlAttributes(spConfig, userName, out pvpVersion, out secClass, out authnContext);

        if (secClass >= 3)
        {
            //ToDO 2nd factor
            throw new ApplicationException($"SecClass {secClass} not yet supported.");
        }
        string relayState = null;
        SAMLIdentityProvider.InitiateSSO(Response, userName, attributes, authnContext, relayState , spName, spConfig.AssertionConsumerServiceUrl);
    }


</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
  
    <form id="Form1" runat="server">
    <div>
      <asp:DropDownList ID="ServiceProviderDropDown" runat="server"></asp:DropDownList>  <asp:Button ID="LoginButton" runat="server" Text="IdP Initiated Login" OnClick="LoginButton_Click" />  
    </div>
    </form>
</body>
</html>
