<%@ Page Language="C#" AutoEventWireup="true" Codebehind="TestPage.aspx.cs" Inherits="TestPvpApplication.TestPage"
  MasterPageFile="~/Headers.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
<br />
<h1>User</h1>
Name:  <asp:Label ID="UserLabel" runat="server"></asp:Label><br />
Authenticated: <asp:CheckBox ID="AuthenticatedCheckBox" runat="server" Enabled="false" /><br />
<h1>Cookie</h1>
  <asp:Label ID="NameLabel" runat="server" AssociatedControlID="NameField">Name</asp:Label>
  <asp:TextBox ID="NameField" runat="server"></asp:TextBox><br />
  <asp:Label ID="ValueLabel" runat="server" AssociatedControlID="ValueField">Value</asp:Label>
  <asp:TextBox ID="ValueField" runat="server"></asp:TextBox><br />
  <asp:Button ID="SetCookieButton" runat="server" Text="Set Cookie" OnClick="SetCookieButton_Click" /><br />
  Domain:
  <asp:Label ID="DomainLabel" runat="server" Text="Label"></asp:Label><br />
  Path:
  <asp:Label ID="PathLabel" runat="server" Text="Label"></asp:Label>
  <h1>Redirect</h1>
  <asp:Button ID="RedirectButton" runat="server" Text="Redirect" OnClick="RedirectButton_Click" />
</asp:Content>
