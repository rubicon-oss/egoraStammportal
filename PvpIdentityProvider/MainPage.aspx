<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainPage.aspx.cs" Inherits="Egora.Stammportal.PvpIdentityProvider.MainPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
  
    <form id="form1" runat="server">
    <div>
      <asp:DropDownList ID="ServiceProviderDropDown" runat="server"></asp:DropDownList>  <asp:Button ID="LoginButton" runat="server" Text="IdP Initiated Login" OnClick="LoginButton_Click" />  
    </div>
    </form>
</body>
</html>
