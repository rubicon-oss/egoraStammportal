using System.Xml.Serialization;

namespace Egora.Stammportal.HttpReverseProxy.CertificateAuthentication
{
  [XmlRoot(Namespace = "http://www.egora.at/Stammportal/CertMap/1.0")]
  public class CertificateAuthenticationConfiguration
  {
    public string HeaderName;

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
    public virtual (string, bool) GetUsername(string certThumbprint, string certSubject, string certIssuer)
    {
      foreach (var mapping in Mappings)
      {
        if (mapping.Thumbprint != null && mapping.Thumbprint == certThumbprint)
          return (mapping.UserName, mapping.IsAdmin);

        if (mapping.Subject != null && mapping.Subject == certSubject && mapping.Issuer != null && mapping.Issuer == certIssuer)
          return (mapping.UserName, mapping.IsAdmin);
      }

      return (null, false);
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
  }
}