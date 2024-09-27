<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="ComponentSpace.SAML2" %>
<%@ Import Namespace="Egora.Stammportal.PvpIdentityProvider" %>
<%@ Import Namespace="ComponentSpace.SAML2.Configuration" %>

<!DOCTYPE html>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        if (SAMLController.Configuration == null)
            SAMLController.Initialize();
        
        var app = Request.QueryString["app"];
        if (!string.IsNullOrEmpty(app))
        {
            var config = SAMLController.Configuration.PartnerServiceProviderConfigurations
                .FirstOrDefault(sp => sp.Name.Equals(app, StringComparison.InvariantCultureIgnoreCase));
            if (config != null)
            {
                InitiateLogin(config);
            }
        }
        
        MessageLabel.Text = $"Die Applikation '{app}' ist nicht konfiguriert.";
    }

    private void InitiateLogin(PartnerServiceProviderConfiguration spConfig)
    {
        string userName = PvpSamlLoginHandler.GetUserName(Request);
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
        SAMLIdentityProvider.InitiateSSO(Response, userName, attributes, authnContext, relayState , spConfig.Name, spConfig.AssertionConsumerServiceUrl);
    }


</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>IdP initiated Login</title>
</head>
<body>
    <form id="Form1" runat="server">
    <div>
        <asp:Label runat="server" ID="MessageLabel"></asp:Label>
        <a href="MainPage.aspx">Zur Auswahl der Applikationen</a>
    </div>
    </form>
</body>
</html>
