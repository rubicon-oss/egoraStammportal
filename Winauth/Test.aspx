<%@ Page Language="C#" AutoEventWireup="true"%>
<%@ Import Namespace="Egora.Stammportal.Authentication" %>

<!DOCTYPE html>

<script runat="server" language="c#">

    public void Page_Load (object sender, EventArgs args)
    {
        var auth = AuthenticationInformation.FromCookie();
        if (auth != null)
        {
                UsernameTextbox.Text = string.Format("Sie sind {0} und haben SecClass {1}.", auth.UserName, auth.SecClass) ;
            
        }
        else
        {
            UsernameTextbox.Text = string.Format("Kein Authentication Ticket {0}.", Settings.Default.LoginCookieName) ;
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
        </div>
    </form>
</body>
</html>
