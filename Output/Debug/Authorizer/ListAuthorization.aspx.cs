using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Web.UI.WebControls;
using Egora.Stammportal.LdapAuthorizationService;

public partial class _ListAuthorization : System.Web.UI.Page
{
  protected ApplicationConfiguration AppConfig;

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      LdapConfiguration config = LdapConfiguration.GetConfiguration();
      foreach (ApplicationConfiguration app in config.Applications)
      {
        ListItem item = new ListItem(app.Name, app.Name);
        ApplicationDropDown.Items.Add(item);
      }
    }

    SetAppConfig();
  }

  private void SetAppConfig()
  {
    LdapConfiguration config = LdapConfiguration.GetConfiguration();
    AppConfig = config.Applications[ApplicationDropDown.SelectedIndex];
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    Page.DataBind();
  }

  public SearchResultCollection GetUsers(string userId, string ldapPath)
  {
    DirectoryEntry root = new DirectoryEntry(ldapPath);
    root.AuthenticationType = AuthenticationTypes.Secure;

    DirectorySearcher search = new DirectorySearcher(root, "samAccountName=" + userId);
    SearchResultCollection coll = search.FindAll();
    return coll;
  }

  private void GetAllUsers(string ldapBase, bool showOnlyUserWithRole)
  {
    List<PvpApplicationLdapAuthorizer> authorizers = new List<PvpApplicationLdapAuthorizer>();
    string urls = AppConfig.WebUrls;
    if (String.IsNullOrEmpty(urls))
      urls = AppConfig.SoapUrls;
    if (!String.IsNullOrEmpty(urls))
    {
      SearchResultCollection allUsers = GetUsers(UserNameFilterTextBox.Text, ldapBase);
      foreach (SearchResult userResult in allUsers)
      {
        DirectoryEntry user = userResult.GetDirectoryEntry();
        string userId = user.Properties["sAMAccountName"].Value.ToString();
        PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer(urls, userId);
        string roles = authorizer.Roles;
        if (!(showOnlyUserWithRole && String.IsNullOrEmpty(roles)))
        {
          authorizers.Add(authorizer);
        }
      }
    }
    authorizers.Sort(new Comparison<PvpApplicationLdapAuthorizer>(CompareAuthorizer));
    AuthorizationGrid.DataSource = authorizers;
    AuthorizationGrid.DataBind();
  }

  private int CompareAuthorizer(PvpApplicationLdapAuthorizer one, PvpApplicationLdapAuthorizer other)
  {
    return String.Compare(one.CommonName, other.CommonName,StringComparison.InvariantCultureIgnoreCase);
  }

  protected void ApplicationDropDown_SelectedIndexChanged(object sender, EventArgs e)
  {
    LdapConfiguration config = LdapConfiguration.GetConfiguration();
    foreach (ApplicationConfiguration app in config.Applications)
    {
      if (app.Name == ApplicationDropDown.SelectedValue)
      {
        AppConfig = app;
        break;
      }
    }
  }

  protected void OuDropDown_SelectedIndexChanged(object sender, EventArgs e)
  {
    UserLdapRootTextBox.Text = OuDropDown.SelectedValue;
  }

  protected void RefreshButton_Click(object sender, EventArgs e)
  {
    GetAllUsers(UserLdapRootTextBox.Text, OnlyUserWithRoleCheckBox.Checked);
  }
}