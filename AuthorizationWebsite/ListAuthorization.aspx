<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ListAuthorization.aspx.cs"
  Inherits="_ListAuthorization" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Pvp Autorisierungen</title>
</head>
<body style="font-family: Arial;">
  <form id="form1" runat="server">
    <div>
      <h1>
        Pvp Autorisierung</h1>
      <b>Autorisierungskonfiguration:
      <asp:DropDownList ID="ApplicationDropDown" runat="server" OnSelectedIndexChanged="ApplicationDropDown_SelectedIndexChanged"
        AutoPostBack="True">
      </asp:DropDownList></b><br />
      <table>
        <tr>
          <td>
            WebUrls:</td>
          <td>
            <asp:Label ID="WebUrls" runat="server" Text='<%#AppConfig.WebUrls%>' /></td>
        </tr>
        <tr>
          <td>
            SoapUrls:</td>
          <td>
            <asp:Label ID="SoapUrls" runat="server" Text='<%#AppConfig.SoapUrls%>' /></td>
        </tr>
        <tr>
          <td>
            geschachtelte Gruppenmitgliedschaft:</td>
          <td>
            <asp:CheckBox ID="RecurseMembership" runat="server" Checked='<%#AppConfig.RecurseGroupMembership%>' Enabled="false"/></td>
        </tr>
        <tr>
          <td>
            Ldap Pfad Benutzer:</td>
          <td>
            <asp:Label ID="LdapRootLabel" runat="server" Text='<%#AppConfig.LdapRoot%>' /></td>
        </tr>
        <tr>
          <td>
            Ldap Pfad Gruppen:</td>
          <td>
            <asp:Label ID="GroupContainerLabel" runat="server" Text='<%#AppConfig.GroupContainer%>' /></td>
        </tr>
        <tr>
          <td>
            Gültigkeit (Sekunden):</td>
          <td>
            <asp:Label ID="TimeToLiveLabel" runat="server" Text='<%#AppConfig.AuthorizationTimeToLive%>' /></td>
        </tr>
        <tr>
          <td>
            Domain:</td>
          <td>
            <asp:Label ID="DomainLabel" runat="server" Text='<%#AppConfig.DomainPrefix%>' /></td>
        </tr>
      </table>
      <br />
      <b>
        Liste der Benutzer und Berechtigungen</b>
      <table>
        <tr>
          <td>
            Ausgangspunkt für die Benutzerliste</td>
          <td>
            <asp:DropDownList ID="OuDropDown" runat="server" OnSelectedIndexChanged="OuDropDown_SelectedIndexChanged"
              AutoPostBack="True">
              <asp:ListItem Selected="True" Text="" Value="" />
              <asp:ListItem Text="rubicon" Value="LDAP://OU=rubicon,DC=int,DC=rubicon-it,DC=com"></asp:ListItem>
              <asp:ListItem Text="test" Value="LDAP://OU=Test,OU=rubicon informationstechnologie gmbh,OU=rubicon,DC=int,DC=rubicon-it,DC=com"></asp:ListItem>
            </asp:DropDownList>
            <asp:TextBox ID="UserLdapRootTextBox" runat="server" Columns="100"></asp:TextBox></td>
        </tr>
        <tr>
          <td>
            Filter für Benutzername:</td>
          <td>
            <asp:TextBox ID="UserNameFilterTextBox" runat="server" Text="*" /></td>
        </tr>
        <tr>
          <td>
            Nur Benutzer mit Pvp-Rolle zeigen</td>
          <td>
            <asp:CheckBox ID="OnlyUserWithRoleCheckBox" runat="server" /></td>
        </tr>
      </table>
      <br />
      <asp:Button ID="RefreshButton" runat="server" Text="Aktualisieren" OnClick="RefreshButton_Click" />
      <br />
      <font size="2em">
        <asp:DataGrid ID="AuthorizationGrid" runat="server" CellPadding="4" ForeColor="#333333"
          GridLines="Vertical" AutoGenerateColumns="False">
          <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
          <EditItemStyle BackColor="#999999" />
          <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
          <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
          <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
          <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
          <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
          <Columns>
            <asp:BoundColumn DataField="CommonName" HeaderText="CN"></asp:BoundColumn>
            <asp:BoundColumn DataField="UserId" HeaderText="UserId"></asp:BoundColumn>
            <asp:BoundColumn DataField="GvGID" HeaderText="GvGID"></asp:BoundColumn>
            <asp:BoundColumn DataField="Mail" HeaderText="Mail"></asp:BoundColumn>
            <asp:BoundColumn DataField="ParticipantId" HeaderText="ParticipantId"></asp:BoundColumn>
            <asp:BoundColumn DataField="Roles" HeaderText="Roles"></asp:BoundColumn>
            <asp:BoundColumn DataField="GvSecClass" HeaderText="SecClass"></asp:BoundColumn>
            <asp:BoundColumn DataField="GvFunction" HeaderText="Funktion"></asp:BoundColumn>
            <asp:BoundColumn DataField="Telephone" HeaderText="Telefon"></asp:BoundColumn>
            <asp:TemplateColumn HeaderText="OU">
              <ItemTemplate>
                OU:<asp:Label runat="server" Text='<%#DataBinder.Eval(Container, "DataItem.Ou")%>'></asp:Label><br />
                GvOuID:<asp:Label runat="server" Text='<%#DataBinder.Eval(Container, "DataItem.GvOuID")%>'></asp:Label><br />
                GvOuDomain:<asp:Label runat="server" Text='<%#DataBinder.Eval(Container, "DataItem.GvOuDomain")%>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Authorize">
              <ItemTemplate>
                OU:<asp:Label runat="server" Text='<%#DataBinder.Eval(Container, "DataItem.AuthorizeOU")%>'></asp:Label><br />
                GvOuID:<asp:Label ID="Label1" runat="server" Text='<%#DataBinder.Eval(Container, "DataItem.AuthorizeGvOuID")%>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateColumn>
            <asp:BoundColumn DataField="InvoiceRecptId" HeaderText="InvoiceRecptId"></asp:BoundColumn>
            <asp:BoundColumn DataField="CostCenterId" HeaderText="CostCenterId"></asp:BoundColumn>
            <asp:TemplateColumn HeaderText="Soap">
              <ItemTemplate>
                <asp:Label runat="server" Text="Xml" ToolTip='<%#DataBinder.Eval(Container, "DataItem.UserPrincipalSoapFragment.OuterXml")%>'></asp:Label>
              </ItemTemplate>
            </asp:TemplateColumn>
          </Columns>
        </asp:DataGrid>
      </font>
    </div>
  </form>
</body>
</html>
