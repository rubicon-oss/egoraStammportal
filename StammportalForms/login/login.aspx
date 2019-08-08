<%@ Page Language="C#" %>
<%@ Import Namespace="System.Web.Security" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">

public void Login_OnClick(object sender, EventArgs args)
{
   if (true)
      FormsAuthentication.RedirectFromLoginPage(UsernameTextbox.Text, false);
   else
     Msg.Text = "Login failed. Please check your user name and password and try again.";
}


</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
  <title>Login</title>
</head>
<body>

<form id="form1" runat="server">
  <h3>Login</h3>

  <asp:Label id="Msg" ForeColor="maroon" runat="server" /><br />

  Username: <asp:Textbox id="UsernameTextbox" runat="server" /><br />
 
  <asp:Button id="LoginButton" Text="Login" OnClick="Login_OnClick" runat="server" />

</form>

</body>
</html>