<%@ Page Language="C#" AutoEventWireup="true"%>

<!DOCTYPE html>

<script runat="server" language="c#">

    public void Page_Load (object sender, EventArgs args)
    {
        UsernameTextbox.Text = HttpContext.Current.User.Identity.Name;
    }


</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            You are <asp:TextBox ID="UsernameTextbox" runat="server"></asp:TextBox>
        </div>
    </form>
</body>
</html>
