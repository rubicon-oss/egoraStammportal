<%@ Page Language="C#" AutoEventWireup="true"  %>
<%@ Import Namespace="Egora.Stammportal.PvpIdentityProvider" %>

<!DOCTYPE html>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.ClientCertificate.IsPresent && Request.ClientCertificate.IsValid)
        {
            var nameParts = Page.Request.ClientCertificate.Subject.Split(new []{' ', ','});
            var user = User.Identity.Name;
            foreach (var namePart in nameParts)
            {
                if (user.ToLowerInvariant().Contains(namePart.ToLowerInvariant()))
                {
                    SecClassHelper.SecClass = 3;
                    break;
                }
            }
        }
        if (!SecClassHelper.SecClass.HasValue)
            SecClassHelper.SecClass = 2;

        var returnUrl = Request.QueryString["RedirectUrl"] ?? "MainPage.aspx";
        Response.Redirect(returnUrl);
        Response.End();
    }
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
