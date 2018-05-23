<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Reset.aspx.cs" Inherits="Egora.Stammportal.HttpReverseProxy.Administration.Reset" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Applications</title>
</head>
<body>
  <form id="form1" runat="server">
    <div>
      <asp:HyperLink ID="ApplicationLink" runat="server" NavigateUrl="Applications.aspx">Applications</asp:HyperLink>
      <asp:HyperLink ID="AuthorizationLink" runat="server" NavigateUrl="Authorization.aspx">Authorization</asp:HyperLink>
      <asp:HyperLink ID="ResetLink" runat="server" NavigateUrl="Reset.aspx">Reset</asp:HyperLink>
      <br />
      Der Proxy wurde zurückgesetzt.
    </div>
  </form>
</body>
</html>
