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
            var auth = new AuthenticationInformation() { SecClass = 2, UserName = Request.LogonUserIdentity.Name };
            Response.Cookies.Add(auth.ToCookie());
            var url = Request.QueryString["ReturnUrl"];
            if (string.IsNullOrEmpty(url))
                url = "Test.aspx";

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