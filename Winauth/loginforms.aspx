<%@ Page Language="C#" %>
<%@ Import Namespace="System.Security.Cryptography" %>
<%@ Import Namespace="System.Web.Security" %>
<%@ Import Namespace="Egora.Stammportal.Authentication" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">

    public void Page_Load(object sender, EventArgs args)
    {
        if (Request.IsAuthenticated && Request.LogonUserIdentity != null && Request.LogonUserIdentity.Name != null)
        {
            string userData = "SecClass2";
            if (Request.QueryString["SecClass"] == "3")
            {
                if (Request.ClientCertificate.IsPresent && Request.ClientCertificate.IsValid)
                {
                    userData = "SecClass3";
                }
                else
                {
                    Msg.Text = "Ein Client Zertifikat ist erforderlich für SecClass 3.";
                    return;
                }
            }
            var ticket = new FormsAuthenticationTicket(4, Request.LogonUserIdentity.Name, DateTime.UtcNow, DateTime.UtcNow + FormsAuthentication.Timeout,
                        false, userData, FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            authCookie.Path = FormsAuthentication.FormsCookiePath;
            authCookie.HttpOnly = true;
            string secure = ConfigurationManager.AppSettings["LoginCookieSecure"];
            if (secure != null && secure.Equals("true", StringComparison.InvariantCultureIgnoreCase) && Request.IsSecureConnection)
                authCookie.Secure = true;
            Response.Cookies.Add(authCookie);

            var url = Request.QueryString["ReturnUrl"];
            if (string.IsNullOrEmpty(url))
                url = "TestForms.aspx";

            Response.Redirect(url, true);
        }
        else
        {
            Msg.Text = "Windows Authentication hat nicht funktioniert.";
        }
    }


</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
  <title>Login</title>
</head>
<body>

<form id="form1" runat="server">
  <h3>Login</h3>
    
    <asp:label runat="server" id="Msg" style="color:maroon"></asp:label>

</form>

</body>
</html>