using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Egora.Pvp.Attributes;
using Egora.Stammportal;
using NUnit.Framework;

namespace Egora.Pvp.Test
{
  [TestFixture]
  public class PvpTokenTest
  {
    [Test]
    public void ChainedTokenTest()
    {
      var headers = GetNameValueCollection19();
      PvpToken token = new PvpToken(headers);
      string result = token.GetChainedSoapFragment().OuterXml;
      Assert.IsTrue(
        result.Contains(
          @"<pvpChainedToken version=""1.9"" xmlns=""http://egov.gv.at/pvp1.xsd"">"))
        ;
      Assert.IsTrue(result.Contains("<participantId>AT:L6:1234789</participantId>"));
      Assert.IsTrue(result.Contains("<userId>mmustermann@kommunalnet.at</userId>"));
      Assert.IsTrue(result.Contains("<cn>Max Mustermann</cn>"));
      Assert.IsTrue(result.Contains("<ou>Meldeamt</ou>"));
      Assert.IsTrue(result.Contains("<gvOuOKZ>AT:GGA-60420-Abt13</gvOuOKZ>"));
      Assert.IsTrue(result.Contains("<gvSecClass>2</gvSecClass>"));
      Assert.IsTrue(result.Contains("<gvGid>AT:B:0:LxXnvpcYZesiqVXsZG0bB==</gvGid>"));
      Assert.IsTrue(result.Contains("<gvOuId>AT:GGA-60420:0815</gvOuId>"));
      Assert.IsTrue(result.Contains("<mail>max.mustermann@hatzendorf.steiermark.at</mail>"));
      Assert.IsTrue(result.Contains("<tel>+43 3155 5153</tel>"));
      Assert.IsTrue(result.Contains("</userPrincipal></authenticate>"));
      Assert.IsTrue(
        result.Contains(@"<role value=""Beispielrolle""><param><key>GKZ</key><value>60420</value></param></role>"));
    }

    private NameValueCollection GetNameValueCollection19()
    {
      NameValueCollection headers = new NameValueCollection()
                                    {
                                      {"X-Version", "1.9"},
                                      {"X-AUTHENTICATE-participantId", "AT:L6:1234789"},
                                      {"X-AUTHENTICATE-UserId", "mmustermann@kommunalnet.at"},
                                      {"X-AUTHENTICATE-cn", "Max Mustermann"},
                                      {"X-AUTHENTICATE-gvGid", "AT:B:0:LxXnvpcYZesiqVXsZG0bB=="},
                                      {"X-AUTHENTICATE-gvOuId", "AT:GGA-60420:0815"},
                                      {"X-AUTHENTICATE-Ou", "Meldeamt"},
                                      {"X-AUTHENTICATE-gvOuOKZ", "AT:GGA-60420-Abt13"},
                                      {"X-AUTHENTICATE-mail", "max.mustermann@hatzendorf.steiermark.at"},
                                      {"X-AUTHENTICATE-tel", "+43 3155 5153"},
                                      {"X-AUTHENTICATE-gvSecClass", "2"},
                                      {"X-AUTHORIZE-roles", "Beispielrolle(GKZ=60420)"},
                                    };
      return headers;
    }

    [Test]
    public void NewToken18Test()
    {
      PvpToken token18 = new PvpToken(PvpVersion.Version18);
      Assert.AreEqual(PvpVersion.Version18, token18.Version);
      var userId = new PvpAttributeUserId();
      userId.Value = "abc";
      token18.Attributes.Add(userId);
      Assert.AreEqual(PvpVersion.Version18, userId.CurrentVersion);
      Assert.AreEqual("abc", userId.Value);
      Assert.AreEqual("X-AUTHENTICATE-UserID", userId.GetHeaderName());
      Assert.AreEqual("1.8", token18.GetAttributeValue(PvpAttributes.VERSION));
    }

    [Test]
    public void OrderTest()
    {
      NameValueCollection headers = new NameValueCollection()
                                    {
                                      {"X-Version", "1.9"},
                                      {"X-AUTHENTICATE-UserId", "mmustermann@kommunalnet.at"},
                                      {"X-AUTHENTICATE-cn", "Max Mustermann"},
                                      {"X-AUTHENTICATE-gvGid", "AT:B:0:LxXnvpcYZesiqVXsZG0bB=="},
                                      {"X-AUTHORIZE-roles", "Beispielrolle(GKZ=60420)"},
                                      {"X-AUTHENTICATE-gvOuId", "AT:GGA-60420:0815"},
                                      {"X-AUTHENTICATE-participantId", "AT:L6:1234789"},
                                      {"X-AUTHENTICATE-Ou", "Meldeamt"},
                                      {"X-AUTHENTICATE-gvOuOKZ", "AT:GGA-60420-Abt13"},
                                      {"X-AUTHENTICATE-mail", "max.mustermann@hatzendorf.steiermark.at"},
                                      {"X-AUTHENTICATE-tel", "+43 3155 5153"},
                                      {"X-AUTHENTICATE-gvSecClass", "2"},
                                    };
      PvpToken token = new PvpToken(headers);
      var httpHeaders = token.GetHeaders();
      Assert.That(httpHeaders[0].Name, Is.EqualTo("X-Version"));
      Assert.That(httpHeaders[1].Name, Is.EqualTo("X-AUTHENTICATE-participantId"));
      Assert.That(httpHeaders[2].Name, Is.EqualTo("X-AUTHENTICATE-UserID"));
      Assert.That(httpHeaders[3].Name, Is.EqualTo("X-AUTHENTICATE-cn"));
      Assert.That(httpHeaders[4].Name, Is.EqualTo("X-AUTHENTICATE-gvOuId"));
      Assert.That(httpHeaders[5].Name, Is.EqualTo("X-AUTHENTICATE-Ou"));
      Assert.That(httpHeaders[6].Name, Is.EqualTo("X-AUTHENTICATE-gvOuOKZ"));
      Assert.That(httpHeaders[7].Name, Is.EqualTo("X-AUTHENTICATE-gvSecClass"));
      Assert.That(httpHeaders[8].Name, Is.EqualTo("X-AUTHENTICATE-mail"));
      Assert.That(httpHeaders[9].Name, Is.EqualTo("X-AUTHENTICATE-tel"));
      Assert.That(httpHeaders[10].Name, Is.EqualTo("X-AUTHENTICATE-gvGid"));
      Assert.That(httpHeaders[11].Name, Is.EqualTo("X-AUTHORIZE-roles"));

      var token21 = token.ConvertTo(PvpVersion.Version21);
      var http21Headers = token21.GetHeaders();
      Assert.That(http21Headers[0].Name, Is.EqualTo("X-PVP-VERSION"));
      Assert.That(http21Headers[1].Name, Is.EqualTo("X-PVP-SECCLASS"));
      Assert.That(http21Headers[2].Name, Is.EqualTo("X-PVP-PRINCIPAL-NAME"));
      Assert.That(http21Headers[3].Name, Is.EqualTo("X-PVP-USERID"));
      Assert.That(http21Headers[4].Name, Is.EqualTo("X-PVP-GID"));
      Assert.That(http21Headers[5].Name, Is.EqualTo("X-PVP-MAIL"));
      Assert.That(http21Headers[6].Name, Is.EqualTo("X-PVP-TEL"));
      Assert.That(http21Headers[7].Name, Is.EqualTo("X-PVP-PARTICIPANT-ID"));
      Assert.That(http21Headers[8].Name, Is.EqualTo("X-PVP-OU-OKZ"));
      Assert.That(http21Headers[9].Name, Is.EqualTo("X-PVP-OU-GV-OU-ID"));
      Assert.That(http21Headers[10].Name, Is.EqualTo("X-PVP-OU"));
      Assert.That(http21Headers[11].Name, Is.EqualTo("X-PVP-ROLES"));

    }

    [Test]
    public void NewToken20Test()
    {
      PvpToken token20 = new PvpToken(PvpVersion.Version20);
      Assert.AreEqual(PvpVersion.Version20, token20.Version);
      var userId = new PvpAttributeUserId();
      userId.Value = "abc";
      token20.Attributes.Add(userId);
      Assert.AreEqual(PvpVersion.Version20, userId.CurrentVersion);
      Assert.AreEqual("abc", userId.Value);
      Assert.AreEqual("X-PVP-USERID", userId.GetHeaderName());
      Assert.AreEqual("2.0", token20.GetAttributeValue(PvpAttributes.VERSION));
    }

    [Test]
    public void Token19Test()
    {
      NameValueCollection headers = new NameValueCollection()
                                      {
                                        {"X-Version", "1.9"},
                                        {"X-AUTHENTICATE-participantId", "AT:L6:1234789"},
                                        {"X-AUTHENTICATE-UserId", "mmustermann@kommunalnet.at"},
                                        {"X-AUTHENTICATE-cn", "Max Mustermann"},
                                        {"X-AUTHENTICATE-gvGid", "AT:B:0:LxXnvpcYZesiqVXsZG0bB=="},
                                        {"X-AUTHENTICATE-gvOuId", "AT:GGA-60420:0815"},
                                        {"X-AUTHENTICATE-Ou", "Meldeamt"},
                                        {"X-AUTHENTICATE-gvOuOKZ", "AT:GGA-60420-Abt13"},
                                        {"X-AUTHENTICATE-mail", "max.mustermann@hatzendorf.steiermark.at"},
                                        {"X-AUTHENTICATE-tel", "+43 3155 5153"},
                                        {"X-AUTHENTICATE-gvSecClass", "2"},
                                        {"X-AUTHORIZE-roles", "Beispielrolle(GKZ=60420)"},
                                        {"X-AUTHORIZE-roles", "Beispielrolle2(ABC=XYZ, DEF=4711)"},
                                      };
      PvpToken token = new PvpToken(headers);
      Assert.AreEqual(PvpVersion.Version19, token.Version);
      Assert.AreEqual("mmustermann@kommunalnet.at", token.GetAttributeValue(PvpAttributes.USERID));
      Assert.AreEqual("Beispielrolle(GKZ=60420);Beispielrolle2(ABC=XYZ, DEF=4711)",
                      token.GetAttributeValue(PvpAttributes.ROLES));
    }

    [Test]
    public void Token21Test()
    {
      NameValueCollection headers = GetNameValueCollection21();
      PvpToken token = new PvpToken(headers);
      Assert.AreEqual(PvpVersion.Version21, token.Version);
      Assert.AreEqual("mmustermann@kommunalnet.at", token.GetAttributeValue(PvpAttributes.USERID));
      Assert.AreEqual("Beispielrolle(GKZ=60420);Beispielrolle2(ABC=XYZ, DEF=4711)",
                      token.GetAttributeValue(PvpAttributes.ROLES));
    }

    [Test]
    public void ConvertTokenTest()
    {
      var headers = GetNameValueCollection21();
      PvpToken token = new PvpToken(headers);
      PvpToken convertedToken = token.ConvertTo(PvpVersion.Version19).ConvertTo(PvpVersion.Version21);
      Assert.AreEqual(PvpVersion.Version21, convertedToken.Version);
      Assert.AreEqual("mmustermann@kommunalnet.at", convertedToken.GetAttributeValue(PvpAttributes.USERID));
      Assert.AreEqual("Beispielrolle(GKZ=60420);Beispielrolle2(ABC=XYZ, DEF=4711)",
                      convertedToken.GetAttributeValue(PvpAttributes.ROLES));
      foreach (HttpHeader header in convertedToken.GetHeaders())
      {
        if (header.Name.Equals("X-PVP-roles",StringComparison.InvariantCultureIgnoreCase))
          continue;

        Assert.AreEqual(headers[header.Name], header.Value, "Fehler bei " + header.Name);
      }
    }

    [Test]
    public void ParseTokenTest()
    {
      NameValueCollection headers = GetNameValueCollection21();
      PvpToken token = new PvpToken(headers);
      PvpToken parsedToken = new PvpToken(token.GetSamlAttributeStatement());

      Assert.AreEqual(PvpVersion.Version21, parsedToken.Version);
      Assert.AreEqual("mmustermann@kommunalnet.at", parsedToken.GetAttributeValue(PvpAttributes.USERID));
      Assert.AreEqual("Beispielrolle(GKZ=60420);Beispielrolle2(ABC=XYZ, DEF=4711)",
                      parsedToken.GetAttributeValue(PvpAttributes.ROLES));
      foreach (HttpHeader header in parsedToken.GetHeaders())
      {
        if (header.Name.Equals("X-PVP-roles", StringComparison.InvariantCultureIgnoreCase))
          continue;

        Assert.AreEqual(headers[header.Name], header.Value, "Fehler bei " + header.Name);
      }
    }

    [Test]
    public void ParseSamlValuesTest()
    {
      NameValueCollection headers = GetNameValueCollection21();
      PvpToken token = new PvpToken(headers);
      XElement statement = XElement.Parse(token.GetSamlAttributeStatement().OuterXml);
      var values = statement.Elements(PvpToken.SamlXNamespace + "Attribute").ToDictionary(a => a.Attribute("Name").Value,
                                                                                          a =>
                                                                                          a.Elements(PvpToken.SamlXNamespace +"AttributeValue").Select(v => v.Value).ToList());
      PvpToken parsedToken = new PvpToken(values);
      Assert.AreEqual(PvpVersion.Version21, parsedToken.Version);
      Assert.AreEqual("mmustermann@kommunalnet.at", parsedToken.GetAttributeValue(PvpAttributes.USERID));
      Assert.AreEqual("Beispielrolle(GKZ=60420);Beispielrolle2(ABC=XYZ, DEF=4711)",
                      parsedToken.GetAttributeValue(PvpAttributes.ROLES));
      foreach (HttpHeader header in parsedToken.GetHeaders())
      {
        if (header.Name.Equals("X-PVP-roles", StringComparison.InvariantCultureIgnoreCase))
          continue;

        Assert.AreEqual(headers[header.Name], header.Value, "Fehler bei " + header.Name);
      }
    }

    [Test]
    [ExpectedException(ExpectedException = typeof(PvpException), ExpectedMessage = "Version muss gesetzt werden.")]
    public void VersionNullTest()
    {
      Dictionary<PvpAttributes, string> values = new Dictionary<PvpAttributes, string>()
                                                   {
                                                     {PvpAttributes.VERSION, null},
                                                     {PvpAttributes.MAIL, "a@nowhere.com"}
                                                   };
      var token =new PvpToken(values, false);
    }

    [Test]
    [ExpectedException(ExpectedException = typeof(PvpException), ExpectedMessage = "Versionsangabe '' nicht gültig.")]
    public void VersionEmptyTest()
    {
      Dictionary<PvpAttributes, string> values = new Dictionary<PvpAttributes, string>()
                                                   {
                                                     {PvpAttributes.VERSION, String.Empty},
                                                     {PvpAttributes.MAIL, "a@nowhere.com"}
                                                   };
      var token = new PvpToken(values, false);
    }

    [Test]
    [ExpectedException(ExpectedException = typeof (PvpException),
      ExpectedMessage = "PVP Versionsinformation 'X-VERSION:1.7' wird nicht unterstützt.")]
    public void InvalidTokenTest()
    {
      NameValueCollection headers = new NameValueCollection()
                                      {
                                        {"X-Version", "1.7"},
                                      };
      PvpToken token = new PvpToken(headers);
    }

    [Test]
    [ExpectedException(ExpectedException = typeof (PvpException), ExpectedMessage = "Es wurde versucht 2 Werte für Attribut GID zu setzen. Dieses Attribut unterstützt aber nur einen Wert.")]
    public void MultipleValueTest()
    {
      NameValueCollection headers = new NameValueCollection()
                                      {
                                        {"X-AUTHENTICATE-gvGid", "test1"},
                                        {"X-AUTHENTICATE-gvGid", "test2"},
                                        {"X-Version", "1.9"},
                                      };
      PvpToken token = new PvpToken(headers);
    }

    [Test]
    [ExpectedException(ExpectedException = typeof(PvpException), ExpectedMessage = "Jeder Attributype darf nur ein Mal vorkommen. Fehler bei PvpAttributeParticipantId")]
    public void MultipleAttributesTest()
    {
      NameValueCollection headers = new NameValueCollection()
                                      {
                                        {"X-Version", "1.9"},
                                        {"X-AUTHENTICATE-participantId", "AT:L6:1234789"},
                                      };
      PvpToken token = new PvpToken(headers);
      PvpAttributeFunction function = new PvpAttributeFunction("SachbearbeiterIn");
      token.Attributes.Add(function);
      Assert.AreEqual(3, token.Attributes.Count);
      Assert.AreEqual("SachbearbeiterIn", token.GetAttributeValue(PvpAttributes.FUNCTION));

      PvpAttributeParticipantId participantId = new PvpAttributeParticipantId("AT:L9:Wien");
      token.Attributes.Add(participantId);
    }

    [Test]
    [ExpectedException(ExpectedException = typeof (PvpException), ExpectedMessage = "Das Attribut CN ist für die Version 2.1 nicht definiert.")]
    public void AttributeNotDefinedTest()
    {
      NameValueCollection headers = new NameValueCollection()
                                      {
                                        {"X-PVP-Version", "2.1"},
                                      };
      PvpToken token = new PvpToken(headers);
      token.Attributes.Add(new PvpAttributeCn("blabla"));
    }

    [Test]
    public void SamlTest()
    {
      PvpToken token = new PvpToken(PvpVersion.Version21);
      token.Attributes.Add(new PvpAttributeUserId("test@egora.at"));
      token.Attributes.Add(new PvpAttributeRoles("Test(A=1)"));
      string xml = token.GetSamlAttributeStatement().OuterXml;
      Assert.IsTrue(xml.StartsWith("<AttributeStatement ID="));
      Assert.IsTrue(xml.EndsWith(" Version=\"2.0\" xmlns=\"urn:oasis:names:tc:SAML:2.0:assertion\"><Attribute Name=\"urn:oid:1.2.40.0.10.2.1.1.261.10\" FriendlyName=\"PVP-VERSION\" NameFormat=\"urn:oasis:names:tc:SAML:2.0:attrname-format:uri\"><AttributeValue>2.1</AttributeValue></Attribute><Attribute Name=\"urn:oid:0.9.2342.19200300.100.1.1\" FriendlyName=\"USERID\" NameFormat=\"urn:oasis:names:tc:SAML:2.0:attrname-format:uri\"><AttributeValue>test@egora.at</AttributeValue></Attribute><Attribute Name=\"urn:oid:1.2.40.0.10.2.1.1.261.30\" FriendlyName=\"ROLES\" NameFormat=\"urn:oasis:names:tc:SAML:2.0:attrname-format:uri\"><AttributeValue>Test(A=1)</AttributeValue></Attribute></AttributeStatement>"));
      //var x="<AttributeStatement ID=\"_aeb030e8-055d-4618-b0df-b8a0dcc4dd60\" Version=\"2.0\" xmlns=\"urn:oasis:names:tc:SAML:2.0:assertion\"><Attribute Name=\"urn:oid:1.2.40.0.10.2.1.1.261.10\" FriendlyName=\"PVP-VERSION\" NameFormat=\"urn:oasis:names:tc:SAML:2.0:attrname-format:uri\"><AttributeValue>2.1</AttributeValue></Attribute><Attribute Name=\"urn:oid:0.9.2342.19200300.100.1.1\" FriendlyName=\"USERID\" NameFormat=\"urn:oasis:names:tc:SAML:2.0:attrname-format:uri\"><AttributeValue>test@egora.at</AttributeValue></Attribute><Attribute Name=\"urn:oid:1.2.40.0.10.2.1.1.261.30\" FriendlyName=\"ROLES\" NameFormat=\"urn:oasis:names:tc:SAML:2.0:attrname-format:uri\"><AttributeValue>Test(A=1)</AttributeValue></Attribute></AttributeStatement>"
    }

    [Test]
    public void SystemPrincipalTest()
    {
      PvpToken token = new PvpToken(GetNameValueCollection19());
      var xml = token.GetSystemPrincipalSoapFragment();
      Assert.AreEqual("<pvpToken version=\"1.9\" xmlns=\"http://egov.gv.at/pvp1.xsd\"><authenticate><participantId>AT:L6:1234789</participantId><systemPrincipal><userId>mmustermann@kommunalnet.at</userId><cn>Max Mustermann</cn><gvOuId>AT:GGA-60420:0815</gvOuId><ou>Meldeamt</ou><gvOuOKZ>AT:GGA-60420-Abt13</gvOuOKZ><gvSecClass>2</gvSecClass></systemPrincipal></authenticate><authorize><role value=\"Beispielrolle\"><param><key>GKZ</key><value>60420</value></param></role></authorize></pvpToken>"
        , xml.OuterXml);
    }

    private NameValueCollection GetNameValueCollection21()
    {
      NameValueCollection headers = new NameValueCollection()
                                      {
                                        {"X-PVP-Version", "2.1"},
                                        {"X-PVP-participant-Id", "AT:L6:1234789"},
                                        {"X-PVP-UserId", "mmustermann@kommunalnet.at"},
                                        {"X-PVP-principal-name", "Mustermann"},
                                        {"X-PVP-given-name", "Max"},
                                        {"X-PVP-Gid", "AT:B:0:LxXnvpcYZesiqVXsZG0bB=="},
                                        {"X-PVP-Ou-gv-Ou-Id", "AT:GGA-60420:0815"},
                                        {"X-PVP-Ou", "Meldeamt"},
                                        {"X-PVP-Ou-OKZ", "AT:GGA-60420-Abt13"},
                                        {"X-PVP-mail", "max.mustermann@hatzendorf.steiermark.at"},
                                        {"X-PVP-tel", "+43 3155 5153"},
                                        {"X-PVP-SecClass", "2"},
                                        {"X-PVP-roles", "Beispielrolle(GKZ=60420)"},
                                        {"X-PVP-roles", "Beispielrolle2(ABC=XYZ, DEF=4711)"},
                                      };
      return headers;
    }

  }
}
