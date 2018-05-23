using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Egora.Pvp.Attributes;
using Egora.Stammportal;

namespace Egora.Pvp
{
  public class PvpToken
  {
    private PvpVersion? _version;

    private static readonly Dictionary<PvpVersion, Dictionary<string, Type>> s_allHeaders =
      new Dictionary<PvpVersion, Dictionary<string, Type>>();

    public static readonly Dictionary<string, Type> SamlAttributeMapping = new Dictionary<string, Type>();

    public static readonly Dictionary<PvpAttributes, Type> KnownAttributeTypes = new Dictionary<PvpAttributes, Type>()
                                                        {
                                                          {PvpAttributes.VERSION, typeof (PvpAttributeVersion)},
                                                          {PvpAttributes.SECCLASS, typeof (PvpAttributeSecClass)},
                                                          {PvpAttributes.PRINCIPAL_NAME, typeof (PvpAttributePrincipalName)},
                                                          {PvpAttributes.GIVEN_NAME, typeof (PvpAttributeGivenName)},
                                                          {PvpAttributes.USERID, typeof (PvpAttributeUserId)},
                                                          {PvpAttributes.GID, typeof (PvpAttributeGid)},
                                                          {PvpAttributes.BPK, typeof (PvpAttributeBpk)},
                                                          {PvpAttributes.ENC_BPK_LIST, typeof (PvpAttributeEncBpkList)},
                                                          {PvpAttributes.MAIL, typeof (PvpAttributeMail)},
                                                          {PvpAttributes.TEL, typeof (PvpAttributeTel)},
                                                          {PvpAttributes.PARTICIPANT_ID, typeof (PvpAttributeParticipantId)},
                                                          {PvpAttributes.PARTICIPANT_OKZ, typeof (PvpAttributeParticipantOkz)},
                                                          {PvpAttributes.OU_OKZ, typeof (PvpAttributeOuOkz)},
                                                          {PvpAttributes.OU_GV_OU_ID, typeof (PvpAttributeOuGvOuId)},
                                                          {PvpAttributes.OU, typeof (PvpAttributeOu)},
                                                          {PvpAttributes.FUNCTION, typeof (PvpAttributeFunction)},
                                                          {PvpAttributes.ROLES, typeof (PvpAttributeRoles)},
                                                          {PvpAttributes.X_AUTHENTICATE_cn, typeof (PvpAttributeCn)},
                                                          {PvpAttributes.X_AUTHORIZE_gvOuId, typeof (PvpAttributeAuthorizeGvOuId)},
                                                          {PvpAttributes.X_AUTHORIZE_Ou, typeof (PvpAttributeAuthorizeOu)},
                                                          {PvpAttributes.X_AUTHORIZE_GvOuOkz, typeof (PvpAttributeAuthorizeOuOkz)},
                                                          {PvpAttributes.COST_CENTER_ID, typeof (PvpAttributeCostCenterId)},
                                                          {PvpAttributes.INVOICE_RECPT_ID, typeof(PvpAttributeInvoiceRecptId)},
                                                          {PvpAttributes.CHARGE_CODE, typeof(PvpAttributeChargeCode)}
                                                        };

    public const string PvpTokenNamespace = "http://egov.gv.at/pvp1.xsd";

    static PvpToken()
    {
      CheckConfiguration();
      InitializeAttributeInfos();
    }

    private static void InitializeAttributeInfos()
    {
      foreach (PvpVersion version in Enum.GetValues(typeof(PvpVersion)))
      {
        Dictionary<string, Type> headers = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        foreach (Type attributeType in KnownAttributeTypes.Values)
        {
          PvpAttribute attribute = (PvpAttribute)Activator.CreateInstance(attributeType);
          string name = attribute.GetHeaderName(version);
          if (name != null)
            headers.Add(name, attributeType);
        }
        s_allHeaders.Add(version, headers);
      }

      foreach (Type attributeType in KnownAttributeTypes.Values)
      {
        PvpAttribute attribute = (PvpAttribute)Activator.CreateInstance(attributeType);
        if (attribute.SamlAttributeName != null)
          SamlAttributeMapping.Add(attribute.SamlAttributeName, attributeType);
      }
    }

    private static void CheckConfiguration()
    {
      List<PvpAttributes> indexes = new List<PvpAttributes>();
      List<string> samlAttributeNames = new List<string>();
      foreach (Type attributeType in KnownAttributeTypes.Values)
      {
        PvpAttribute attribute = (PvpAttribute)Activator.CreateInstance(attributeType);

        if (indexes.Contains(attribute.Index))
          throw new PvpInitializationException("Zwei Attribute mit gleichem Index gefunden: " +
                                              attribute.Index.ToString("G") + ".");
        indexes.Add(attribute.Index);

        if (attribute.SamlAttributeName != null)
        {
          if (samlAttributeNames.Contains(attribute.SamlAttributeName))
            throw new PvpInitializationException("Zwei Attribute mit gleichem SamlAttributeName gefunden: " +
                                                attribute.SamlAttributeName + ".");
          samlAttributeNames.Add(attribute.SamlAttributeName);
        }
      }
    }

    private PvpToken()
    {
      _attributes.CollectionChanged += Attributes_CollectionChanged;
    }

    public PvpToken(PvpVersion version)
      : this()
    {
      _version = version;
      Attributes.Add(new PvpAttributeVersion());
    }

    public PvpToken(NameValueCollection httpHeaders)
      : this()
    {
      if (httpHeaders == null)
        throw new PvpException("Die übergebenen Values dürfen nicht null sein, sie müssen zumindest den Versionsheader beinhalten.");

      _version = DeterminePvpVersion(httpHeaders);
      if (!_version.HasValue)
        throw new PvpException("Pvp Version konnte nicht ermittelt werden.");

      ParseHeaders(httpHeaders);
    }

    public PvpToken(XmlElement attributeStatement)
      : this()
    {
      ParseSaml(attributeStatement);
    }

    public PvpToken(Dictionary<string, List<string>> samlAttributeValues)
      : this()
    {
      ParseSamlValues(samlAttributeValues);
    }

    private void ParseSamlValues(Dictionary<string, List<string>> samlAttributeValues)
    {
      List<string> versions = null;
      if (!samlAttributeValues.TryGetValue(new PvpAttributeVersion().SamlAttributeName, out versions))
        throw new PvpException("Version muss gesetzt werden.");

      string version = versions[0];
      if (!PvpVersionNumber.PvpVersions.ContainsKey(version))
        throw new PvpVersionNotSupportedException("Versionsangabe '" + version + "' nicht gültig.", version);
      _version = PvpVersionNumber.PvpVersions[version];

      foreach (PvpAttribute attr in samlAttributeValues.Select(CreatePvpAttribute).Where(a => a != null))
        Attributes.Add(attr);
    }

    private PvpAttribute CreatePvpAttribute(KeyValuePair<string, List<string>> samlAttributeValue)
    {
      if (SamlAttributeMapping.ContainsKey(samlAttributeValue.Key))
      {
        PvpAttribute pvpAttribute =
          (PvpAttribute)Activator.CreateInstance(SamlAttributeMapping[samlAttributeValue.Key]);
        pvpAttribute.SetValues(samlAttributeValue.Value.ToArray());
        return pvpAttribute;
      }
      //throw new PvpException("Unknown SAML Attribute '" + samlAttributeValue.Key + "'.");
      Trace.WriteLine("Ignoring unknown SAML Attribute '" + samlAttributeValue.Key + "'.");
      return null;
    }

    public PvpToken(Dictionary<PvpAttributes, string> values, bool ignoreInvalidAttributes)
      : this()
    {
      string version;
      if (!values.TryGetValue(PvpAttributes.VERSION, out version) || version == null)
        throw new PvpException("Version muss gesetzt werden.");

      if (!PvpVersionNumber.PvpVersions.ContainsKey(version))
        throw new PvpVersionNotSupportedException("Versionsangabe '" + version + "' nicht gültig.", version);
      _version = PvpVersionNumber.PvpVersions[version];

      foreach (KeyValuePair<PvpAttributes, string> pair in values)
      {
        Type t;
        if (!KnownAttributeTypes.TryGetValue(pair.Key, out t))
        {
          if (ignoreInvalidAttributes)
          {
            Trace.WriteLine("Ignoring unknown Attribute " + pair.Key.ToString("G"));
          }
          else
          {
            throw new PvpInitializationException("Attribut nicht bekannt: " + pair.Key);
          }
        }

        var attr = (PvpAttribute)Activator.CreateInstance(t);
        if (attr.IsAvailableInVersion(_version.Value))
        {
          attr.Value = pair.Value;
          Attributes.Add(attr);
        }
        else if (ignoreInvalidAttributes)
        {
          Trace.WriteLine("Ignoring incompatible Attribute " + pair.Key.ToString("G"));
        }
        else
        {
          throw new PvpInitializationException("Attribut nicht für Version " + version + " zulässig: " + pair.Key);
        }
      }
    }

    private void ParseSaml(XmlElement attributeStatement)
    {
      XElement statement = XElement.Load(new XmlNodeReader(attributeStatement));
      string version =
      statement.Elements(SamlXNamespace + "Attribute")
      .First(a => a.Attribute("Name").Value == new PvpAttributeVersion().SamlAttributeName)
      .Elements(SamlXNamespace + "AttributeValue").First().Value;

      if (!PvpVersionNumber.PvpVersions.ContainsKey(version))
        throw new PvpVersionNotSupportedException("Versionsangabe " + version + " nicht gültig.", version);

      _version = PvpVersionNumber.PvpVersions[version];

      var attributes = statement.Elements(SamlXNamespace + "Attribute")
               .Where(a => SamlAttributeMapping.ContainsKey(a.Attribute("Name").Value))
               .Select(CreatePvpAttribute);

      foreach (var attribute in attributes)
        Attributes.Add(attribute);
    }

    private PvpAttribute CreatePvpAttribute(XElement attribute)
    {
      Type t = SamlAttributeMapping[attribute.Attribute("Name").Value];
      PvpAttribute pvpAttribute = (PvpAttribute)Activator.CreateInstance(t);
      pvpAttribute.SetValues(attribute.Elements(SamlXNamespace + "AttributeValue").Select(e => e.Value).ToArray());
      return pvpAttribute;
    }

    private void ParseHeaders(NameValueCollection values)
    {
      Dictionary<string, Type> headers = s_allHeaders[Version];
      foreach (string headerName in values.Keys)
      {
        Type t;
        if (headers.TryGetValue(headerName, out t))
        {
          string[] strings = values.GetValues(headerName);
          PvpAttribute pvpAttribute = (PvpAttribute)Activator.CreateInstance(t);
          pvpAttribute.SetValues(strings);
          Attributes.Add(pvpAttribute);
        }
        else
        {
          Trace.WriteLine("Header " + headerName + " ignored.");
        }
      }
    }

    private readonly ObservableCollection<PvpAttribute> _attributes = new ObservableCollection<PvpAttribute>();

    public ObservableCollection<PvpAttribute> Attributes
    {
      get { return _attributes; }
    }

    private void Attributes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      foreach (var a in e.NewItems)
      {
        PvpAttribute attribute = a as PvpAttribute;
        if (attribute != null && _version.HasValue)
          attribute.CurrentVersion = _version.Value;
      }

      var multiple =
        Attributes.GroupBy(a => a.GetType())
                  .Select(group => new { Type = group.Key, Count = group.Count() })
                  .Where(g => g.Count > 1)
                  .Select(g => g.Type.Name);
      if (multiple.Any())
        throw new PvpException("Jeder Attributype darf nur ein Mal vorkommen. Fehler bei " + String.Join(",", multiple));

      if (_version.HasValue)
      {
        var offender =
          Attributes.GroupBy(a => a.GetHeaderName(_version.Value))
                    .Select(group => new { Name = group.Key, Count = group.Count() })
                    .Where(x => x.Count > 1).Select(y => y.Name);
        if (offender.Any())
          throw new PvpException("Keine zwei Attribute dürfen den gleichen Headernamen haben. Fehler bei " + String.Join(",", offender));
      }
    }

    public virtual string GetAttributeValue(PvpAttributes attribute)
    {
      var attr = GetPvpAttribute(attribute);
      if (attr != null)
        return attr.Value;

      return null;
    }

    public virtual PvpAttribute GetPvpAttribute(PvpAttributes attribute)
    {
      PvpAttribute attr = Attributes.SingleOrDefault(a => a.Index == attribute);
      return attr;
    }

    public string ParticipantId
    {
      get { return GetAttributeValue(PvpAttributes.PARTICIPANT_ID); }
    }

    public PvpAttributeRoles RoleAttribute
    {
      get
      {
        return (PvpAttributeRoles)GetPvpAttribute(PvpAttributes.ROLES);
      }
    }

    public string VersionAsString
    {
      get { return GetAttributeValue(PvpAttributes.VERSION); }
    }

    public PvpVersion Version
    {
      get
      {
        if (!_version.HasValue)
          throw new PvpException("PvpToken nicht richtig initialisiert, Version nicht bekannt.");

        return _version.Value;
      }
    }

    public static PvpVersion? DeterminePvpVersion(NameValueCollection values)
    {
      foreach (var header in new List<string>() { "X-PVP-VERSION", "X-PVP-EGOVTOKEN-VERSION", "X-VERSION" })
      {
        var val = values.Get(header);
        if (String.IsNullOrEmpty(val))
          continue;

        if (PvpVersionNumber.PvpVersions.ContainsKey(val))
          return PvpVersionNumber.PvpVersions[val];

        throw new PvpVersionNotSupportedException("PVP Versionsinformation '" + header + ":" + val +
                                                  "' wird nicht unterstützt.", val);
      }

      return null;
    }

    public XmlElement GetChainedSoapFragment()
    {
      string pvpToken = String.Format(PvpChainedTokenFormat,
                                      EncodingUtil.XmlEncode(VersionAsString),
                                      EncodingUtil.XmlEncode(ParticipantId),
                                      GetSoapUserPrincipalFragment(),
                                      GetSoapAuthorizeFragment());

      XmlDocument doc = new XmlDocument();
      doc.LoadXml(pvpToken);

      return doc.DocumentElement;
    }

    public XmlElement GetUserPrincipalSoapFragment()
    {
      string pvpToken = String.Format(PvpTokenFormat,
                                      EncodingUtil.XmlEncode(VersionAsString),
                                      EncodingUtil.XmlEncode(ParticipantId),
                                      GetSoapUserPrincipalFragment(),
                                      GetSoapAuthorizeFragment());

      XmlDocument doc = new XmlDocument();
      doc.LoadXml(pvpToken);

      return doc.DocumentElement;
    }

    public XmlElement GetSystemPrincipalSoapFragment()
    {
      string pvpToken = String.Format(PvpTokenFormat,
                                      EncodingUtil.XmlEncode(VersionAsString),
                                      EncodingUtil.XmlEncode(ParticipantId),
                                      GetSoapSystemPrincipalFragment(),
                                      GetSoapAuthorizeFragment());

      XmlDocument doc = new XmlDocument();
      doc.LoadXml(pvpToken);

      return doc.DocumentElement;
    }
    public PvpAttributes[] SoapUserPrincipalAttributes = new PvpAttributes[]
                                                       {
                                                         PvpAttributes.USERID,
                                                         PvpAttributes.X_AUTHENTICATE_cn,
                                                         PvpAttributes.OU_GV_OU_ID,
                                                         PvpAttributes.OU,
                                                         PvpAttributes.OU_OKZ,
                                                         PvpAttributes.SECCLASS,
                                                         PvpAttributes.MAIL,
                                                         PvpAttributes.TEL,
                                                         PvpAttributes.GID,
                                                         PvpAttributes.FUNCTION,
                                                         PvpAttributes.BPK,
                                                       };

    public PvpAttributes[] SoapSystemPrincipalAttributes = new PvpAttributes[]
                                                       {
                                                         PvpAttributes.USERID,
                                                         PvpAttributes.X_AUTHENTICATE_cn,
                                                         PvpAttributes.OU_GV_OU_ID,
                                                         PvpAttributes.OU,
                                                         PvpAttributes.OU_OKZ,
                                                         PvpAttributes.SECCLASS,
                                                       };

    public PvpAttributes[] SoapAuthorizeAttributes = new PvpAttributes[]
                                                       {
                                                         PvpAttributes.X_AUTHORIZE_gvOuId,
                                                         PvpAttributes.X_AUTHORIZE_Ou,
                                                         PvpAttributes.X_AUTHORIZE_GvOuOkz,
                                                         PvpAttributes.ROLES,
                                                       };

    private string GetSoapUserPrincipalFragment()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("<userPrincipal>");
      sb.Append(String.Join(Environment.NewLine, SoapUserPrincipalAttributes.Select(index => GetXmlPart(index))));
      sb.AppendLine("</userPrincipal>");
      return sb.ToString();
    }

    private string GetSoapSystemPrincipalFragment()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("<systemPrincipal>");
      sb.Append(String.Join(Environment.NewLine, SoapSystemPrincipalAttributes.Select(index => GetXmlPart(index))));
      sb.AppendLine("</systemPrincipal>");
      return sb.ToString();
    }

    private string GetSoapAuthorizeFragment()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(String.Join(Environment.NewLine, SoapAuthorizeAttributes.Select(index => GetXmlPart(index))));
      return sb.ToString();
    }

    private string GetXmlPart(PvpAttributes index)
    {
      var attribute = GetPvpAttribute(index);
      if (attribute != null)
        return attribute.GetXmlPart();
      return null;
    }

    protected string PvpChainedTokenFormat
    {
      get
      {
        string chainedTokenFormat = null;

        switch (Version)
        {
          case PvpVersion.Version18:
          case PvpVersion.Version19:
            chainedTokenFormat =
              @"<pvpChainedToken version=""{0}"" xmlns=""" + PvpTokenNamespace + @""" >
<authenticate>
<participantId>{1}</participantId>
{2}
</authenticate>
<authorize>
{3}
</authorize>
</pvpChainedToken>";

            break;

          case PvpVersion.Version20:
          case PvpVersion.Version21:
            throw new PvpException("ChainedToken nur bis Version 1.9 verfügbar.");
        }

        return chainedTokenFormat;
      }
    }

    protected string PvpTokenFormat
    {
      get
      {
        string tokenFormat = null;

        switch (Version)
        {
          case PvpVersion.Version18:
          case PvpVersion.Version19:
            tokenFormat =
              @"<pvpToken version=""{0}"" xmlns=""" + PvpTokenNamespace + @""" >
<authenticate>
<participantId>{1}</participantId>
{2}
</authenticate>
<authorize>
{3}
</authorize>
</pvpToken>";

            break;

          case PvpVersion.Version20:
          case PvpVersion.Version21:
            throw new PvpException("PvpToken nur bis Version 1.9 verfügbar.");
        }

        return tokenFormat;
      }
    }


    public bool IsInRole(string pvpRoleName, NameValueCollection roleParameters)
    {
      return RoleAttribute != null && RoleAttribute.HasRole(new PvpRole(pvpRoleName, roleParameters));
    }

    public PvpRole[] GetRoles()
    {
      if (RoleAttribute == null)
        return null;

      return RoleAttribute.Roles.ToArray();
    }

    public PvpRole GetRole(string roleName)
    {
      if (RoleAttribute == null)
        return null;

      return RoleAttribute.GetRole(roleName);
    }

    public bool IsInRole(string role)
    {
      return RoleAttribute != null && RoleAttribute.HasRole(new PvpRole(role));
    }

    public string UserId
    {
      get { return GetAttributeValue(PvpAttributes.USERID); }
    }

    public string Mail
    {
      get { return GetAttributeValue(PvpAttributes.MAIL); }
    }

    public string Phone
    {
      get { return GetAttributeValue(PvpAttributes.TEL); }
    }

    public string GvGid
    {
      get { return GetAttributeValue(PvpAttributes.GID); }
    }

    public string GvFunction
    {
      get { return GetAttributeValue(PvpAttributes.FUNCTION); }
    }

    public string[] Bpk
    {
      get
      {
        var bpk = GetAttributeValue(PvpAttributes.BPK);
        if (bpk == null)
          return null;

        return bpk.Split(',');
      }
    }

    public List<HttpHeader> GetHeaders()
    {
      return Attributes.OrderBy(a => a.Order).Select(a => new HttpHeader(a.GetHeaderName(), a.Value)).ToList();
    }

    public const string SamlNamespace = "urn:oasis:names:tc:SAML:2.0:assertion";
    public static readonly XNamespace SamlXNamespace = XNamespace.Get(SamlNamespace);
    private const string CnSeparator = "  ";

    public XmlElement GetSamlAttributeStatement()
    {
      XmlDocument document = new XmlDocument();
      document.AppendChild(GetAttributeStatementXmlElement(document));
      return document.DocumentElement;
    }

    private XmlElement GetAttributeStatementXmlElement(XmlDocument assertion)
    {
      XmlElement attributeStatement = assertion.CreateElement(String.Empty, "AttributeStatement", SamlNamespace);
      XmlAttribute id = assertion.CreateAttribute("ID");
      id.Value = "_" + Guid.NewGuid();
      attributeStatement.SetAttributeNode(id);
      XmlAttribute version = assertion.CreateAttribute("Version");
      version.Value = "2.0";
      attributeStatement.Attributes.Append(version);
      foreach (var pvpAttribute in Attributes)
      {
        XmlElement samlAttribute = pvpAttribute.GetSamlAttribute(assertion);
        if (samlAttribute != null)
          attributeStatement.AppendChild(samlAttribute);
      }
      return attributeStatement;
    }

    public virtual PvpToken ConvertTo(PvpVersion version)
    {
      PvpToken newToken = new PvpToken(version);
      foreach (PvpAttribute attribute in Attributes)
      {
        Type attributeType = attribute.GetType();

        if (attributeType == typeof(PvpAttributeVersion))
          continue;

        if (version == PvpVersion.Version18 || version == PvpVersion.Version19)
        {
          if (attributeType == typeof(PvpAttributeGivenName))
            continue;

          if (attributeType == typeof(PvpAttributePrincipalName))
          {
            string cn = GetAttributeValue(PvpAttributes.PRINCIPAL_NAME);
            string givenName = GetAttributeValue(PvpAttributes.GIVEN_NAME);
            if (!String.IsNullOrEmpty(givenName))
              cn += CnSeparator + givenName;

            newToken.Attributes.Add(new PvpAttributeCn(cn));
            continue;
          }
        }

        if (version == PvpVersion.Version20 || version == PvpVersion.Version21)
        {
          if (attributeType == typeof(PvpAttributeCn))
          {
            string cn = GetAttributeValue(PvpAttributes.X_AUTHENTICATE_cn);
            int index = cn.IndexOf(CnSeparator);
            if (index > 0)
            {
              string givenName = cn.Substring(index + 2);
              newToken.Attributes.Add(new PvpAttributeGivenName(givenName));
              cn = cn.Substring(0, index);
            }
            newToken.Attributes.Add(new PvpAttributePrincipalName(cn));
            continue;
          }
        }

        if (attribute.IsAvailableInVersion(version))
        {
          PvpAttribute newAttribute = (PvpAttribute)Activator.CreateInstance(attributeType);
          newAttribute.Value = attribute.Value;
          newToken.Attributes.Add(newAttribute);
        }
      }

      return newToken;
    }
  }
}
