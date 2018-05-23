<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Applications.aspx.cs" Inherits="Egora.Stammportal.HttpReverseProxy.Administration.Applications" %>

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
      <asp:HyperLink ID="ConnectionLink" runat="server" NavigateUrl="Connection.aspx">Connections</asp:HyperLink> 
      <asp:HyperLink ID="ResetLink" runat="server" NavigateUrl="Reset.aspx">Reset</asp:HyperLink>
      <br />
      <table runat="server" id="AppTable">
        <tr>
          <td style="vertical-align: top; padding-right: 10px;">
            <asp:GridView ID="ApplicationsGridView" runat="server" OnSelectedIndexChanged="ApplicationsGridView_SelectedIndexChanged"
              CellPadding="4" ForeColor="#333333" GridLines="None">
              <Columns>
                <asp:CommandField SelectText="Details" ShowSelectButton="True" />
              </Columns>
              <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
              <RowStyle BackColor="#EFF3FB" />
              <EditRowStyle BackColor="#2461BF" />
              <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
              <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
              <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
              <AlternatingRowStyle BackColor="White" />
            </asp:GridView>
          </td>
          <td style="vertical-align: top; padding-right: 10px;">
            <asp:DetailsView ID="InfoDetailsView" runat="server" CellPadding="4" ForeColor="#333333"
              GridLines="None">
              <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
              <CommandRowStyle BackColor="#D1DDF1" Font-Bold="True" />
              <EditRowStyle BackColor="#2461BF" />
              <RowStyle BackColor="#EFF3FB" />
              <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
              <FieldHeaderStyle BackColor="#DEE8F5" Font-Bold="True" />
              <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
              <AlternatingRowStyle BackColor="White" />
            </asp:DetailsView>
          </td>
        </tr>
      </table>
      <h2>
        History</h2>
      <asp:GridView ID="HistoryGridView" runat="server" CellPadding="4" ForeColor="#333333"
        GridLines="None" AutoGenerateColumns="false">
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="#2461BF" />
        <RowStyle BackColor="#EFF3FB" />
        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="White" />
        <Columns>
          <asp:BoundField DataField="Date" HeaderText="Date" />
          <asp:BoundField DataField="Path" HeaderText="ProxyPath" />
          <asp:BoundField DataField="Method" HeaderText="Method" />
          <asp:BoundField DataField="Target" HeaderText="Target Url" />
        </Columns>
      </asp:GridView>
    </div>
  </form>
</body>
</html>
