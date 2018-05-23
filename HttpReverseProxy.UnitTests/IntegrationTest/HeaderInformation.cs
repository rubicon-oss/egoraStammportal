/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System.Xml.Serialization;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests.IntegrationTest
{
  public class HeaderInformation
  {
    private string _name;
    private string _value;

    public HeaderInformation()
    {
    }

    public HeaderInformation(string name, string value)
    {
      Value = value;
      Name = name;
    }

    [XmlElement()]
    public string Value
    {
      get { return _value; }
      set { _value = value; }
    }

    [XmlAttribute()]
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public override string ToString()
    {
      return Name + ";" + Value;
    }
  }
}