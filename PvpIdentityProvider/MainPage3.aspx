<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="ComponentSpace.SAML2" %>
<%@ Import Namespace="Egora.Stammportal.PvpIdentityProvider" %>
<%@ Import Namespace="ComponentSpace.SAML2.Configuration" %>
<%@ Import Namespace="System.Xml.Serialization" %>

<!DOCTYPE html>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        if (SAMLController.Configuration == null)
            SAMLController.Initialize();
        var s = new XmlSerializer(typeof(EntityConfiguration));
        var config = (EntityConfiguration) s.Deserialize(File.OpenText("Entities3.xml"));
        var entities = config.Entities.ToList();
        
        if (SAMLController.Configuration != null
            && SAMLController.Configuration.PartnerServiceProviderConfigurations != null
            && SAMLController.Configuration.PartnerServiceProviderConfigurations.Count > 0)
        {
            foreach (var serviceProviderConfiguration in SAMLController.Configuration.PartnerServiceProviderConfigurations)
            {
                var entity = entities.FirstOrDefault(_ => _.EntityID == serviceProviderConfiguration.Name);
                if (entity != null)
                {
                    if (entity.SecClass >= 3)
                    {
                        if (!SecClassHelper.SecClass.HasValue )
                        {
                            Response.Redirect("Login3.aspx?RedirectUrl=MainPage.aspx");
                            Response.End();
                        }
                            
                        if (SecClassHelper.SecClass < 3)
                            continue;
                    }
                    
                    ServiceProviderDropDown.Items.Add(new ListItem(
                        string.IsNullOrEmpty(entity.FriendlyName) ? serviceProviderConfiguration.Name : entity.FriendlyName, 
                        serviceProviderConfiguration.AssertionConsumerServiceUrl));
                }
            }
        }
    }

    protected void LoginButton_Click(object sender, EventArgs e)
    {
        var spName = ServiceProviderDropDown.SelectedItem.Value;
        var spConfig = SAMLController.Configuration.PartnerServiceProviderConfigurations
            .First(sp => sp.AssertionConsumerServiceUrl.Equals(spName));

        try
        {
            InitiateLogin(spConfig);
        }
        catch (Exception exception)
        {
            MessageLabel.Text = "Es trat folgender Fehler auf: " + exception.Message;
        }
    }

    private void InitiateLogin(PartnerServiceProviderConfiguration spConfig)
    {
        string userName = PvpSamlLoginHandler.GetUserName(Request);
        string pvpVersion;
        int secClass;
        string authnContext;
        var attributes = PvpSamlLoginHandler.GetSamlAttributes(spConfig, userName, out pvpVersion, out secClass, out authnContext);

        if (secClass >= 3 && (!SecClassHelper.SecClass.HasValue || SecClassHelper.SecClass < secClass))
        {
            //ToDO 2nd factor
            throw new ApplicationException("SecClass " + secClass + "not yet supported.");
        }
        string relayState = null;
        SAMLIdentityProvider.InitiateSSO(Response, userName, attributes, authnContext, relayState , spConfig.Name, spConfig.AssertionConsumerServiceUrl);
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
        <asp:Label runat="server" ID="MessageLabel"></asp:Label>
    </form>
</body>
</html>
