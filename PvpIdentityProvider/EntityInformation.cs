using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Egora.Stammportal.PvpIdentityProvider
{
  public class EntityInformation
  {
    [XmlAttribute]
    public string EntityID { get; set; }
    [XmlAttribute]
    public string FriendlyName { get; set; }
    [XmlAttribute]
    public int SecClass { get; set; }
    [XmlAttribute]
    public string SpInitiatedUrl { get; set; }
  }

  [XmlRoot]
  public class EntityConfiguration
  {
    [XmlElement]
    public EntityInformation[] Entities { get; set; }
  }
}