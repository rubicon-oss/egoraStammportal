<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StartPage.aspx.cs" Inherits="TestPvpApplication.Saml.StartPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <asp:LinkButton runat="server" OnClick="SPLogin_Click" runat="server" ID="SPLogin">Anmelden SP-Initiated</asp:LinkButton>
    </div>
    </form>
</body>
</html>
