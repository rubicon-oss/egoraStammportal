<%@ Page Language="C#" %>
<%@ Import Namespace="System.DirectoryServices.AccountManagement" %>
<%@ Import Namespace="System.Web.Security" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">

    public void Login_OnClick(object sender, EventArgs args)
    {
        bool isValid;
        using(PrincipalContext pc = new PrincipalContext(ContextType.Domain, "RUBICON"))
        {
            // validate the credentials
            isValid = pc.ValidateCredentials(UsernameTextbox.Text, PasswordTextbox.Text);
        }
        if (isValid)
        {
            Msg.Text = String.Empty;
            FormsAuthentication.RedirectFromLoginPage(UsernameTextbox.Text, false);
        }
        else
        {
            Msg.Text = "Login failed. Please check your user name and password and try again.";
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
<table>
    <tr><td>Username: </td><td><asp:Textbox id="UsernameTextbox" runat="server" /></td></tr>
    <tr><td>Password: </td><td><asp:Textbox id="PasswordTextbox" runat="server"  TextMode="Password"/></td></tr>
</table>

  <asp:Button id="LoginButton" Text="Login" OnClick="Login_OnClick" runat="server" />

</form>

</body>
</html>