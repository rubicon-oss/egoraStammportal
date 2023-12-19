using System;
using System.Collections.Specialized;
using System.Data;
using System.Configuration;
using System.Text;
using System.Web;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Xml;
using Egora.Pvp;
using Egora.Pvp.Attributes;

namespace Egora.Stammportal.LdapAuthorizationService
{
  public class PvpApplicationLdapAuthorizer
  {
    public PvpApplicationLdapAuthorizer(string rootUrl, string userId)
      : this (rootUrl, userId, LdapConfiguration.GetConfiguration ())
    {
    }

    public PvpApplicationLdapAuthorizer(string rootUrl, string userId, LdapConfiguration configuration)
    {
      if (rootUrl == null)
        throw new ArgumentNullException ("rootUrl");

      if (userId == null)
        throw new ArgumentNullException ("userId");

      if (configuration == null)
        throw new ArgumentNullException ("configuration");

      _application = configuration.GetApplication (rootUrl);
      if (_application != null)
      {
        if (!String.IsNullOrEmpty (_application.DomainPrefix) 
          && userId.StartsWith (_application.DomainPrefix + @"\", StringComparison.InvariantCultureIgnoreCase))
        {
          userId = userId.Remove (0, _application.DomainPrefix.Length + 1);
        }
      }
      _userId = userId;
      _rootUrl = rootUrl;
      _isValid = (_application != null) && _userId != null && User != null 
                && (!_application.MustHaveRole || !String.IsNullOrEmpty(String.Join(",", _application.GetAttributeValue(PvpAttributes.ROLES, User, Groups))));
    }

    protected bool HasRole
    {
      get { return !String.IsNullOrEmpty(GetAttributeValue(PvpAttributes.ROLES)); }
    }

    private bool _isValid = false;
    public bool IsValid
    {
      get { return _isValid; }
    }

    public bool IsWeb
    {
      get { return IsValid && _application.IsWeb (_rootUrl); }
    }

    public bool IsSoap
    {
      get { return IsValid && _application.IsSoap (_rootUrl); }
    }

    public bool IsSaml
    {
      get { return IsValid && _application.IsSaml(_rootUrl); }
    }

    private string _userId;
    public string UserId
    {
      get { return _userId; }
      set { _userId = value; }
    }
    
    private string _rootUrl;
    private ApplicationConfiguration _application;
    private DirectoryEntry _user;
    private bool _userFetched = false;
    public DirectoryEntry User
    {
      get
      {
        if (_user == null && !_userFetched)
        {
          _user = GetOneUser (_userId);
          _userFetched = true;
        }

        return _user;
      }
    }

    private string _mail;
    public string Mail
    {
      get
      {
        if (_mail == null)
          _mail = GetAttributeValue (PvpAttributes.MAIL);

        return _mail;
      }
    }

    private string _pvpUserId;
    public string PvpUserId
    {
      get
      {
        if (_pvpUserId == null)
          _pvpUserId = GetAttributeValue(PvpAttributes.USERID);

        return _pvpUserId;
      }
    }

    public string ApplicationName
    {
      get { return _application == null ? null : _application.Name; }  
    }

    private string _commonName;
    public string CommonName
    {
      get
      {
        if (_commonName == null)
        {
          _commonName = GetAttributeValue (PvpAttributes.X_AUTHENTICATE_cn);
        }

        return _commonName;
      }
    }

    private PvpToken _pvpToken = null;
    public PvpToken GetPvpToken()
    {
      if (_pvpToken == null)
        _pvpToken=new PvpToken(GetValueCollection(), true);
      return _pvpToken;
    }

    public List<HttpHeader> GetHeaders()
    {
      return GetPvpToken().GetHeaders();
    }

    public XmlElement GetSamlAttributeStatement()
    {
      return GetPvpToken().GetSamlAttributeStatement();
    }

    private Dictionary<PvpAttributes, string> GetValueCollection()
    {
      var values = new Dictionary<PvpAttributes, string>();
      foreach (PvpAttributes attribute in _application.AllDefinedPvpAttributes)
      {
        string attributeValue = GetAttributeValue(attribute);
        if (!String.IsNullOrEmpty(attributeValue))
          values.Add(attribute, attributeValue);
      }
      return values;
    }

    public string GetAttributeValue(PvpAttributes attribute)
    {
      if (User == null || !IsValid)
        return null;

      string[] values = _application.GetAttributeValue (attribute, User, Groups);
      
      if (values == null)
        return null;

      return attribute==PvpAttributes.ROLES ? String.Join(";", values) : String.Join (",", values);
    }

    private XmlElement _userPrincipalSoapFragment = null;
    public XmlElement UserPrincipalSoapFragment
    {
      get
      {
        if (User==null)
          return null;

        if (_userPrincipalSoapFragment == null)
        {
          _userPrincipalSoapFragment=GetPvpToken().GetUserPrincipalSoapFragment();
        }

        return _userPrincipalSoapFragment;
      }
    }

    public string ParticipantID
    {
      get { return GetAttributeValue (PvpAttributes.PARTICIPANT_ID); }
    }

    public string GvGID
    {
      get { return GetAttributeValue (PvpAttributes.GID); }
    }

    public string GvOuID
    {
      get { return GetAttributeValue (PvpAttributes.OU_GV_OU_ID); }
    }

    public string Ou
    {
      get { return GetAttributeValue (PvpAttributes.OU); }
    }

    public string GvFunction
    {
      get { return GetAttributeValue (PvpAttributes.FUNCTION); }
    }

    public int AuthorizationTimeToLive
    {
      get { return _application.AuthorizationTimeToLive; }
    }

    public string Telephone
    {
      get { return GetAttributeValue (PvpAttributes.TEL); }
    }

    public string Version
    {
      get { return GetAttributeValue (PvpAttributes.VERSION); }
    }

    public string GvSecClass
    {
      get { return GetAttributeValue (PvpAttributes.SECCLASS); }
    }

    public string AuthorizeGvOuID
    {
      get { return GetAttributeValue (PvpAttributes.X_AUTHORIZE_gvOuId); }
    }

    public string AuthorizeOU
    {
      get { return GetAttributeValue (PvpAttributes.X_AUTHORIZE_Ou); }
    }

    public string CostCenterId
    {
      get { return GetAttributeValue(PvpAttributes.COST_CENTER_ID); }
    }

    public string ChargeCode
    {
      get { return GetAttributeValue(PvpAttributes.CHARGE_CODE); }
    }

    public string InvoiceRecptId
    {
      get { return GetAttributeValue(PvpAttributes.INVOICE_RECPT_ID); }
    }

    private List<DirectoryEntry> _groups;
    private List<DirectoryEntry> Groups
    {
      get
      {
        if (_groups == null)
        {
          List<string> memberNames = new List<string> ();
          _groups = GetAllPvpApplicationGroups (User, null, memberNames);
        }
        return _groups;
      }
    }

    public string Roles
    {
      get
      {
        PvpAttributeRoles roles = GetPvpToken().RoleAttribute;
        return roles==null ? null : roles.Value;
      }
    }

    private static Hashtable s_groupCache = new Hashtable ();

    private List<DirectoryEntry> GetAllPvpApplicationGroups(DirectoryEntry user, DirectoryEntry group, List<string> memberNames)
    {
      DirectoryEntry member = user;
      if (member == null)
        member = group;
      List<DirectoryEntry> groups = new List<DirectoryEntry> ();
      if (member == null)
        return groups;
      string memberDistinguishedName = member.Properties["distinguishedName"].Value.ToString();

      string key = memberDistinguishedName + _application.GroupContainer;
      if (user == null && Properties.Settings.Default.CacheGroupResolution)
      {
        if (s_groupCache.ContainsKey (key))
          return (List<DirectoryEntry>)s_groupCache[key];
      }

      if (memberNames.Contains (memberDistinguishedName))
        return groups; 
      memberNames.Add (memberDistinguishedName);

      foreach (SearchResult result in GetPvpApplicationGroups (memberDistinguishedName))
      {
        groups.Add (result.GetDirectoryEntry ());
      }

      if (_application.RecurseGroupMembership)
      {
        foreach (string groupName in member.Properties["memberOf"])
        {
          DirectoryEntry root = new DirectoryEntry (_application.LdapRoot);
          root.AuthenticationType = AuthenticationTypes.Secure;

          DirectorySearcher search = new DirectorySearcher (
              root,
              String.Format (Properties.Settings.Default.GroupFilter, groupName));
          SearchResultCollection coll = search.FindAll ();

          if (coll.Count > 1)
            throw new ApplicationException (String.Format ("More than one object {0} found.", groupName));

          if (coll.Count == 1)
          {
            groups.AddRange (GetAllPvpApplicationGroups (null, coll[0].GetDirectoryEntry (), memberNames));
          }
        }
      }

      if (user == null && Properties.Settings.Default.CacheGroupResolution)
      {
        s_groupCache[key] = groups;
      }

      return groups;
    }

    private SearchResultCollection GetPvpApplicationGroups(string member)
    {
      DirectoryEntry zmrGroupContainer = new DirectoryEntry (_application.GroupContainer);
      zmrGroupContainer.AuthenticationType = AuthenticationTypes.Secure;

      DirectorySearcher search = new DirectorySearcher (
          zmrGroupContainer,
          String.Format (Properties.Settings.Default.ApplicationGroupFilter, member));
      SearchResultCollection coll = search.FindAll ();

      return coll;
    }

    public SearchResultCollection GetUsers(string userId)
    {
      DirectoryEntry root = new DirectoryEntry (_application.LdapRoot);
      root.AuthenticationType = AuthenticationTypes.Secure;

      DirectorySearcher search = new DirectorySearcher (root, String.Format (Properties.Settings.Default.UserFilter, userId));
      SearchResultCollection coll = search.FindAll ();
      return coll;
    }

    private DirectoryEntry GetOneUser(string userId)
    {
      SearchResultCollection coll = GetUsers (userId);

      if (coll.Count > 1)
        throw new ApplicationException (String.Format ("More than one User {0} found.", userId));

      if (coll.Count < 1)
        return null;

      DirectoryEntry user = coll[0].GetDirectoryEntry ();
      if (_application.UserProperties != null)
        user.RefreshCache (_application.UserProperties.Split(','));
      return user;
    }

  }
}
