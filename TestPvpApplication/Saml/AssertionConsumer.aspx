<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssertionConsumer.aspx.cs" Inherits="TestPvpApplication.Saml.AssertionComsumer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <asp:TextBox ID="UserNameTextBox" runat="server" Width="100%"></asp:TextBox>
      <br/>
      <asp:TextBox ID="SamlMessageTextBox" runat="server" Width="100%" Height="700" TextMode="MultiLine"></asp:TextBox>
    </div>
    </form>
</body>
</html>
