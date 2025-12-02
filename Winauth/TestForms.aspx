<%@ Page Language="C#" AutoEventWireup="true"%>
<%@ Import Namespace="System.Security.Principal" %>

<!DOCTYPE html>

<script runat="server" language="c#">

    public void Page_Load (object sender, EventArgs args)
    {
        UsernameTextbox.Text = User.Identity.Name;
        var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        if (cookie != null)
        {
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            UserRoleTextBox.Text = ticket.UserData;
        }
    }


</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="UsernameTextbox" runat="server"></asp:TextBox>
            <asp:TextBox ID="UserRoleTextBox" runat="server"></asp:TextBox>
        </div>
    </form>
</body>
</html>
