using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;

namespace Egora.Pvp.Attributes
{
  public class PvpAttribute
  {
    private PvpVersion? _currentVersion;
    private readonly PvpVersion[] _availableInVersions;
    private readonly string _friendlyName;
    private readonly string _samlAttributeName;
    private readonly PvpAttributes _index;
    private readonly Dictionary<PvpVersion, string> _headerNames;
    private readonly string _soapElementName;
    private string _value;

    protected PvpAttribute(string friendlyName, PvpAttributes index, string samlAttributeName, PvpVersion[] availableInVersions, Dictionary<PvpVersion, string> headerNames, string soapElementName)
    {
      _friendlyName = friendlyName;
      _index = index;
      _samlAttributeName = samlAttributeName;
      _availableInVersions = availableInVersions;
      _headerNames = headerNames;
      _soapElementName = soapElementName;
      
      _availableInVersions.Where(
        v =>
          {
            if (!_headerNames.ContainsKey(v))
              throw new PvpInitializationException("PvpAttribute " + FriendlyName + " für Version " + v.ToString("G") + " definiert, aber kein HeaderName für diese Version definiert.");
            return false;
          }).ToList();

      _headerNames.Keys.Where(
        v =>
          {
            if (!_availableInVersions.Contains(v))
              throw new PvpInitializationException("HeaderName für Version " + v.ToString("G") + " definiert, aber PvpAttribute " + FriendlyName + " nicht für diese Version definiert.");
            return false;
          }).ToList();
    }

    public PvpVersion[] AvailableInVersions
    {
      get { return _availableInVersions; }
    }

    public virtual PvpVersion CurrentVersion
    {
      get
      {
        if (_currentVersion.HasValue)
          return _currentVersion.Value;

        throw new PvpException("CurrentVersion of PvpAttribute " + FriendlyName + " not set.");
      }
      
      internal set
      {
        if (AvailableInVersions.Contains(value))
          _currentVersion = value;
        else
          throw new PvpVersionNotSupportedException("Das Attribut " + FriendlyName + " ist für die Version " + PvpVersionNumber.PvpVersions.Single(pair => pair.Value==value).Key + " nicht definiert.", value.ToString("G"));
      }
    }

    public virtual bool IsAvailableInVersion(PvpVersion version)
    {
      return (AvailableInVersions != null) && AvailableInVersions.Contains(version);
    }

    public string FriendlyName
    {
      get { return _friendlyName; }
    }

    public string SamlAttributeName
    {
      get { return _samlAttributeName; }
    }

    public string SoapElementName
    {
      get { return _soapElementName; }
    }

    public PvpAttributes Index
    {
      get { return _index; }
    }

    protected Dictionary<PvpVersion, string> HeaderNames
    {
      get { return _headerNames; }
    }

    public virtual string GetHeaderName(PvpVersion version)
    {
      string name = null;
      HeaderNames.TryGetValue(version, out name);
      return name;
    }

    public virtual string GetHeaderName()
    {
      return GetHeaderName(CurrentVersion);
    }

    public virtual string Value
    {
      get { return _value; }
      set
      {
        CheckValue(value);
        _value = value;
      }
    }

    public virtual void SetValues(string[] values)
    {
      if (values == null)
      {
        Value = null;
      }
      else if (values.Length == 1)
      {
        Value = values[0];
      }
      else
      {
        throw new PvpException("Es wurde versucht " + values.Length.ToString() + " Werte für Attribut " + FriendlyName + " zu setzen. Dieses Attribut unterstützt aber nur einen Wert.");
      }
    }

    public virtual void CheckValue(string value)
    {
    }

    protected void StringMaxLenCheck(string value, int maxLen)
    {
      if (value!=null && value.Length>maxLen)
        throw new PvpException("Wert des PvpAttributes " + FriendlyName + " darf maximal " + maxLen.ToString() + " lang sein.");
    }

    public virtual string GetXmlPart()
    {
      if (Value != null)
        return String.Format("<{0}>{1}</{0}>", SoapElementName, EncodingUtil.XmlEncode(Value));
      
      return null;
    }

    public virtual XmlElement GetSamlAttribute(XmlDocument assertion)
    {
      if (SamlAttributeName == null)
        return null;

      XmlElement attribute = assertion.CreateElement(String.Empty, "Attribute", PvpToken.SamlNamespace);
      
      XmlAttribute name = assertion.CreateAttribute("Name");
      name.Value = SamlAttributeName;
      attribute.Attributes.Append(name);
      
      XmlAttribute friendlyName = assertion.CreateAttribute("FriendlyName");
      friendlyName.Value = FriendlyName;
      attribute.Attributes.Append(friendlyName);
      
      XmlAttribute nameFormat = assertion.CreateAttribute("NameFormat");
      nameFormat.Value = "urn:oasis:names:tc:SAML:2.0:attrname-format:uri";
      attribute.Attributes.Append(nameFormat);
      
      XmlElement attributeValue = assertion.CreateElement(String.Empty, "AttributeValue", PvpToken.SamlNamespace);
      attributeValue.InnerText = Value;
      attribute.AppendChild(attributeValue);
      
      return attribute;
    }

    private static Dictionary<PvpAttributes, int> s_index19;

    private static Dictionary<PvpAttributes, int> Index19
    {
      get
      {
        if (s_index19 == null)
        {
          var d = new Dictionary<PvpAttributes, int>();
          for (int i = 0; i < PvpLegacy.AttributeOrder19.Count; i++)
          {
            d.Add(PvpLegacy.AttributeOrder19[i], i+1);
          }
          s_index19 = d;
        }
        return s_index19;
      }
    }

    public virtual int Order
    {
      get
      {
        int i;
        if (CurrentVersion.Equals(PvpVersion.Version18) || CurrentVersion.Equals(PvpVersion.Version19))
        {
          if (!Index19.TryGetValue(Index, out i))
            i = 999;
        }
        else
        {
          i = (int) Index;
        }
        return i;
      }
    }
  }

}
