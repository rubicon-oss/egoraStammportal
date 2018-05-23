<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CloseConnection.aspx.cs" Inherits="TestPvpApplication.CloseConnection" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    This page closes the connection every second request. Now 
      <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label> 
      <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="CloseConnection.aspx">Try Again</asp:HyperLink>
    </div>
    </form>
</body>
</html>
