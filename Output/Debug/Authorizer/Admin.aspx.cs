using System;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using Egora.Stammportal.LdapAuthorizationService;

public partial class _Admin : System.Web.UI.Page
{
  protected void Page_PreRender(object sender, EventArgs e)
  {
    LdapConfiguration config = LdapConfiguration.GetConfiguration();

    foreach (ApplicationConfiguration app in config.Applications)
    {
      TableRow row = new TableRow();
      TableCell nameCell = new TableCell();
      nameCell.Text = app.Name;
      row.Cells.Add(nameCell);

      TableCell attrCell = new TableCell();
      StringBuilder attrSb = new StringBuilder();
      foreach (XmlAttribute attr in app.AllAttributes)
      {
        attrSb.Append(Server.HtmlEncode(String.Format("{0}={1} ", attr.LocalName, attr.Value)));
        attrSb.Append("<br />");
      }
      attrCell.Text = attrSb.ToString();
      row.Cells.Add(attrCell);

      TableCell propCell = new TableCell();
      StringBuilder propSb = new StringBuilder();
      if (app.PvpConfigAttributes != null)
      {
        foreach (PvpConfigAttribute pvpAttr in app.PvpConfigAttributes)
        {
          propSb.Append(Server.HtmlEncode(
                          String.Format("{0}: Source={1:G} Ldap={2}, Default={3}, Format={4} Formatter={5}",
                                        pvpAttr.Name, pvpAttr.Source, pvpAttr.LdapAttribute,
                                        pvpAttr.DefaultValue, pvpAttr.Format, pvpAttr.Formatter)));
          propSb.Append("<br />");
        }
      }
      propCell.Text = propSb.ToString();
      row.Cells.Add(propCell);

      ConfigTable.Rows.Add(row);
    }
  }

  protected void ReloadButton_Click(object sender, EventArgs e)
  {
    LdapConfiguration.GetConfiguration(true);
  }
}