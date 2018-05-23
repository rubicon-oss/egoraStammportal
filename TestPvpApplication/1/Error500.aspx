<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error500.aspx.cs" Inherits="TestPvpApplication.Error500" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    This page responds with http status code 500 every second request.
      <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="Error500.aspx">Try Again</asp:HyperLink>
    </div>
    </form>
</body>
</html>
