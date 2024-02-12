<%@ Page Language="C#" %>
<%@ Import Namespace="System.Web.Security" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">

    public void Logout_OnClick(object sender, EventArgs args)
    {
        //var cookieValue = "";
        //var cookie = new HttpCookie(Config.AuthenticationCookieName, cookieValue);
        //cookie.Expires = new DateTime(1900, 1, 1);
        //Response.Cookies.Add(cookie);
    }

</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
  <title>Logout</title>
</head>
<body>

<form id="form1" runat="server">
  <h3>Logout</h3>
  <asp:Label runat="server" id="Msg" style="color: maroon"></asp:Label>  
  <asp:Button id="LogoutButton" Text="Logout" OnClick="Logout_OnClick" runat="server" />

</form>

</body>
</html>