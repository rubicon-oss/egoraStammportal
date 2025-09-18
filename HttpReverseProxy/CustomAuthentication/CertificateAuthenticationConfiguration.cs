using System;
using System.Xml.Serialization;

namespace Egora.Stammportal.HttpReverseProxy.CustomAuthentication
{
  [XmlRoot(Namespace = "http://www.egora.at/Stammportal/CertMap/1.0")]
  public class CertificateAuthenticationConfiguration
  {
    public string HeaderName;
    public string SecClass;

    [XmlArray]
    [XmlArrayItem(Type = typeof(CertificateMapping))]
    public CertificateMapping[] Mappings;

    public static CertificateAuthenticationConfiguration CreateFromFile(string fileName)
    {
      XmlSerializer serializer = new XmlSerializer(typeof(CertificateAuthenticationConfiguration));
      CertificateAuthenticationConfiguration mapping;
      using (System.IO.StreamReader reader = new System.IO.StreamReader(fileName))
      {
        mapping = (CertificateAuthenticationConfiguration)serializer.Deserialize(reader);
      }

      return mapping;
    }
    public virtual (string userName, bool isAdmin, string secClass) GetUserInfo(string certThumbprint, string certSubject, string certIssuer)
    {
      foreach (var mapping in Mappings)
      {
        if (mapping.Thumbprint != null && mapping.Thumbprint.Equals(certThumbprint,StringComparison.InvariantCultureIgnoreCase))
          return (mapping.UserName, mapping.IsAdmin, mapping.SecClass ?? SecClass);

        if (mapping.Subject != null && mapping.Subject == certSubject && mapping.Issuer != null && mapping.Issuer == certIssuer)
          return (mapping.UserName, mapping.IsAdmin, mapping.SecClass ?? SecClass);
      }

      return (null, false, null);
    }
  }
  public class CertificateMapping
  {
    [XmlAttribute]
    public string Thumbprint;
    [XmlAttribute]
    public string Subject;
    [XmlAttribute]
    public string Issuer;
    [XmlAttribute]
    public string UserName;
    [XmlAttribute]
    public bool IsAdmin;
    [XmlAttribute]
    public string SecClass;
  }
}