<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="_Admin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <style>
   td
   { 
     vertical-align:top;
   }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <asp:Button ID="ReloadButton" runat="server" Text="Reload Configuration" OnClick="ReloadButton_Click" />
      <br /><br />
      <asp:Table ID="ConfigTable" runat="server" BorderStyle="Solid" BorderWidth="1px" CellPadding="1" CellSpacing="1" GridLines="Both">
      <asp:TableHeaderRow>
      <asp:TableHeaderCell>Application</asp:TableHeaderCell>
      <asp:TableHeaderCell>Settings</asp:TableHeaderCell>
      <asp:TableHeaderCell Width="50%">PvpAttributes</asp:TableHeaderCell>
      </asp:TableHeaderRow>
      </asp:Table>
    </div>
    </form>
</body>
</html>
