<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HeaderTestPage.aspx.cs" Inherits="HeaderTestPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Testpage</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

	<script runat="server" language="C#">

    public void Page_Load(object sender, System.EventArgs e)
    {
      foreach (string headerName in Request.Headers.AllKeys)
      {
        TableRow row = new TableRow();
        TableCell nameCell = new TableCell();
        nameCell.Text = headerName;
        TableCell valueCell = new TableCell();
        valueCell.Text = Request.Headers[headerName];
        row.Cells.Add(nameCell);
        row.Cells.Add(valueCell);
        HeaderTable.Rows.Add(row);
      }
      if (Request.ClientCertificate != null)
      {
        foreach (string keyName in Request.ClientCertificate.AllKeys)
        {
          TableRow row = new TableRow();
          TableCell nameCell = new TableCell();
          nameCell.Text = keyName;
          TableCell valueCell = new TableCell();
          valueCell.Text = Request.ClientCertificate[keyName];
          row.Cells.Add(nameCell);
          row.Cells.Add(valueCell);
          CertificateTable.Rows.Add(row);
        }
      }
    }

    </script>
			<h1>PVP Testseite</h1>
			<h2>Header</h2>
            <asp:Table id="HeaderTable" runat="server">
			</asp:Table>
			<h2>Zertifikat</h2>
			<asp:Table id="CertificateTable" runat="server">
			</asp:Table>
    </div>
    </form>
</body>
</html>
