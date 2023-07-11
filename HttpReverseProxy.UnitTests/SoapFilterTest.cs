/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System.Collections.Specialized;
using System.Xml;
using Egora.Pvp;
using Egora.Stammportal.HttpReverseProxy.Mapping;
using Egora.Stammportal.HttpReverseProxy.StreamFilter;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests
{
  [TestFixture]
  public class SoapFilterTest
  {
    [Test]
    public void HeaderForSoapWithoutHeaderElement()
    {
      var doc = GetSoapBodyOnly();

      SoapFilter filter = new SoapFilter(null, 1000, PvpTokenHandling.remove, null, null);
      XmlElement header = filter.SelectOrCreateHeader(doc.DocumentElement);

      Assert.IsNotNull(header, "Header");
      Assert.AreEqual("Header", header.LocalName);
      Assert.AreEqual("http://schemas.xmlsoap.org/soap/envelope/", header.NamespaceURI);
      Assert.AreSame(header.OwnerDocument, doc);

      XmlElement body =
        (XmlElement) doc.GetElementsByTagName("Body", "http://schemas.xmlsoap.org/soap/envelope/")[0];
      Assert.AreSame(body, header.NextSibling);
    }

    [Test]
    public void HeaderForSoapWithHeaderElement()
    {
      var doc = GetSoapWithHeaderAndSecurity();

      SoapFilter filter = new SoapFilter(null, 1000, PvpTokenHandling.remove, null, null);
      XmlElement header = filter.SelectOrCreateHeader(doc.DocumentElement);

      Assert.IsNotNull(header, "Header");
      Assert.AreEqual("Header", header.LocalName);
      Assert.AreEqual("http://schemas.xmlsoap.org/soap/envelope/", header.NamespaceURI);
      Assert.AreSame(header.OwnerDocument, doc);
    }

    [Test]
    public void SecurityForSoapWithoutHeaderElement()
    {
      var doc = GetSoapBodyOnly();

      SoapFilter filter = new SoapFilter(null, 1000, PvpTokenHandling.remove, null, null);
      XmlElement securityElement = filter.SelectOrCreateSecurityElement(doc.DocumentElement);

      Assert.IsNotNull(securityElement, "Security");
      Assert.AreEqual("Security", securityElement.LocalName);
      Assert.AreEqual("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", securityElement.NamespaceURI);
      Assert.AreSame(securityElement.OwnerDocument, doc);
    }

    [Test]
    public void SecurityCustomForSoapWithoutHeaderElement()
    {
      var doc = GetSoapBodyOnly();

      SoapFilter filter = new SoapFilter(null, 1000, PvpTokenHandling.remove, null, "SomeStupidNamespace");
      XmlElement securityElement = filter.SelectOrCreateSecurityElement(doc.DocumentElement);

      Assert.IsNotNull(securityElement, "Security");
      Assert.AreEqual("Security", securityElement.LocalName);
      Assert.AreEqual("SomeStupidNamespace", securityElement.NamespaceURI);
      Assert.AreSame(securityElement.OwnerDocument, doc);
    }

    [Test]
    public void SecurityForSoapWithHeaderElement()
    {
      var doc = GetSoapWithHeaderAndSecurity();

      SoapFilter filter = new SoapFilter(null, 1000, PvpTokenHandling.remove, null, null);
      XmlElement securityElement = filter.SelectOrCreateSecurityElement(doc.DocumentElement);

      Assert.IsNotNull(securityElement, "Security");
      Assert.AreEqual("Security", securityElement.LocalName);
      Assert.AreEqual("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", securityElement.NamespaceURI);
      Assert.AreSame(securityElement.OwnerDocument, doc);

      XmlElement security =
        (XmlElement)doc.GetElementsByTagName("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")[0];
      Assert.AreSame(security, securityElement);
    }

    [Test]
    public void PvpTokenForSoapWithHeaderElement()
    {
      var doc = GetSoapWithPvpToken();

      CustomAuthorization authorization = new CustomAuthorization();
      XmlDocument authDoc = new XmlDocument();
      var authToken = GetAuthToken19(authDoc);

      authorization.SoapHeaderXmlFragment = authToken;
      SoapFilter filter = new SoapFilter(authorization, 1000, PvpTokenHandling.remove, null, null);
      XmlElement pvpToken = filter.InsertAuthorization(doc);

      Assert.IsNotNull(pvpToken);
      Assert.AreEqual(authToken.InnerXml, pvpToken.InnerXml);

      XmlElement envelope = filter.SelectXmlElement(doc.DocumentElement, "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
      Assert.IsNotNull(envelope);

      Assert.AreEqual(pvpToken.OuterXml, pvpToken.ParentNode.InnerXml);
    }

    [Test]
    public void PvpTokenForSoapWithToken()
    {
            var doc = GetSoapWithPvpToken();

            CustomAuthorization authorization = new CustomAuthorization();
            XmlDocument authDoc = new XmlDocument();
            var authToken = GetAuthToken18(authDoc);

            authorization.SoapHeaderXmlFragment = authToken;
            authorization.PvpVersion = "1.8";
            SoapFilter filter = new SoapFilter(authorization, 1000, PvpTokenHandling.remove, null, null);
            XmlElement pvpToken = filter.InsertAuthorization(doc);

            Assert.IsNotNull(pvpToken);
            Assert.AreEqual(authToken.InnerXml, pvpToken.InnerXml);

            XmlElement envelope = filter.SelectXmlElement(doc.DocumentElement, "Envelope",
                                                          "http://schemas.xmlsoap.org/soap/envelope/");
            Assert.IsNotNull(envelope);

            Assert.AreEqual(pvpToken.OuterXml, pvpToken.ParentNode.InnerXml);
        }

    [Test]
    public void SecurityElement18Test()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(EnvStart +
              @"<S:Header>
<wsse:Security>
</wsse:Security>
</S:Header>
<S:Body>
</S:Body>
</S:Envelope>");

            CustomAuthorization authorization18 = new CustomAuthorization();
            XmlDocument authDoc = new XmlDocument();
            XmlElement authToken = authDoc.CreateElement("P:pvpToken", "http://egov.gv.at/pvp1.xsd");
            authToken.SetAttribute("version", "1.8");
            authToken.InnerXml =
              @"<authenticate>
<participantId>AT:L6:994</participantId>
<userPrincipal>
<userId>fmeier@stmk.gv.at</userId>
<cn>F. Meier</cn>
<gvOuId>AT:L6:1299</gvOuId>
<ou>L6AL-F2/c</ou>
<mail>fmeier@stmk.gv.at</mail>
<tel>fmeier@stmk.gv.at</tel>
<gvSecClass>2</gvSecClass>
<gvGid>AT:B:0:UhO5RG++klaOTsVY+CU=</gvGid>
<gvFunction>SB</gvFunction>
</userPrincipal>
</authenticate>
<authorize>
<role value=""ZMR-Fremdenbehoerdenanfrage"">
<param>
<key>GKZ</key>
<value>60100</value>
</param>
</role>
</authorize>";

            authorization18.SoapHeaderXmlFragment = authToken;
            authorization18.PvpVersion = "1.8";
            SoapFilter filter = new SoapFilter(authorization18, 1000, PvpTokenHandling.remove, null, null);
            var security = filter.SelectOrCreateSecurityElement(doc.DocumentElement);
            Assert.AreEqual("http://schemas.xmlsoap.org/ws/2002/04/secext", security.NamespaceURI);
        }

    [Test]
    public void SecurityElement18Chain19Test()
    {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(EnvStart +
@"
<S:Header>
<Security xmlns=""http://schemas.xmlsoap.org/ws/2002/04/secext"">
			<pvpToken version=""1.8"" xmlns=""http://egov.gv.at/pvp1.xsd"">
				<authenticate>
					<participantId>AT:VKZ:XFN-216900x</participantId>
					<userPrincipal>
						<userId>RUBICON\werner.kugler</userId>
						<cn>RUBICON\werner.kugler</cn>
						<gvOuId>633243</gvOuId>
						<ou>rubicon ZMR Test</ou>
						<gvSecClass>2</gvSecClass>
						<gvGid>RUBICON\werner.kugler</gvGid>
					</userPrincipal>
				</authenticate>
				<authorize>
					<role value=""ZMR-Behoerdenanfrage""/>
				</authorize>
			</pvpToken>
		</Security> 
</S:Header>
<S:Body>
</S:Body>
</S:Envelope> 
" 
        );

      CustomAuthorization authorization19 = new CustomAuthorization();
      XmlDocument authDoc = new XmlDocument();
      XmlElement authToken = authDoc.CreateElement("P:pvpToken", "http://egov.gv.at/pvp1.xsd");
      authToken.SetAttribute("version", "1.9");
      authToken.InnerXml =
        @"<authenticate>
					<participantId>egora</participantId>
					<systemPrincipal>
						<userId>test</userId>
						<cn>test</cn>
						<gvOuId>egora</gvOuId>
						<ou>rubicon PD</ou>
						<gvSecClass>3</gvSecClass>
					</systemPrincipal>
				</authenticate>
   <authorize>
<role value=""ZMR-Behoerdenanfrage"">
</role>
</authorize>";

      authorization19.SoapHeaderXmlFragment = authToken;
      authorization19.PvpVersion = "1.9";
      SoapFilter filter = new SoapFilter(authorization19, 1000, PvpTokenHandling.chain, null, null);
      var security = filter.InsertAuthorization(doc);
      Assert.That(security, Is.Not.Null);
      Assert.That(security.InnerXml, Text.Contains("pvpChainedToken").IgnoreCase);
    }
    [Test]
    public void SecurityElement19Test()
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(EnvStart +
            @"<S:Header>
<seleg:Security>
</seleg:Security>
</S:Header>
<S:Body>
</S:Body>
</S:Envelope>");

        CustomAuthorization authorization19 = new CustomAuthorization();
        XmlDocument authDoc = new XmlDocument();
        XmlElement authToken = authDoc.CreateElement("P:pvpToken", "http://egov.gv.at/pvp1.xsd");
        authToken.SetAttribute("version", "1.9");
        authToken.InnerXml =
            @"<authenticate>
<participantId>AT:L6:994</participantId>
<userPrincipal>
<userId>fmeier@stmk.gv.at</userId>
<cn>F. Meier</cn>
<gvOuId>AT:L6:1299</gvOuId>
<ou>L6AL-F2/c</ou>
<mail>fmeier@stmk.gv.at</mail>
<tel>fmeier@stmk.gv.at</tel>
<gvSecClass>2</gvSecClass>
<gvGid>AT:B:0:UhO5RG++klaOTsVY+CU=</gvGid>
<gvFunction>SB</gvFunction>
</userPrincipal>
</authenticate>
<authorize>
<role value=""ZMR-Fremdenbehoerdenanfrage"">
<param>
<key>GKZ</key>
<value>60100</value>
</param>
</role>
</authorize>";

        authorization19.SoapHeaderXmlFragment = authToken;
        authorization19.PvpVersion = "1.9";
        SoapFilter filter = new SoapFilter(authorization19, 1000, PvpTokenHandling.remove, null, null);
        var security = filter.SelectOrCreateSecurityElement(doc.DocumentElement);
        Assert.AreEqual("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", security.NamespaceURI);
    }
    [Test]
    public void PvpTokenWithChainedTokenForSoapWithoutHeaderElement()
    {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(
        @"<S:Envelope
xmlns:P=""http://egov.gv.at/pvp1.xsd""
xmlns:S=""http://schemas.xmlsoap.org/soap/envelope/""

xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
<S:Body>
</S:Body>
</S:Envelope>");

      CustomAuthorization authorization = new CustomAuthorization();
      XmlDocument authDoc = new XmlDocument();
      XmlElement authToken = authDoc.CreateElement("P:pvpToken", "http://egov.gv.at/pvp1.xsd");
      authToken.SetAttribute("version", "1.9");
      authToken.InnerXml =
        @"<authenticate>
<participantId>AT:L6:994</participantId>
<systemPrincipal>
  <userId>egovstar.appserv1.intra.xyz.gv.at</userId>
  <cn>Anwendung 1 Register-Interface</cn>
  <gvOuId>AT:L6:4711</gvOuId>
  <ou>Fachabteilung 1B Informationstechnik</ou>
  <gvOuOKZ>AT:L6-FA1B</gvOuOKZ>
  <gvSecClass>2</gvSecClass>
</systemPrincipal>
</authenticate>
<authorize>
  <role value=""Registerabfrage""/>
</authorize>
<pvpChainedToken version=""1.9"">
  <authenticate>
  <userPrincipal>
    <userId>fmeier@stmk.gv.at</userId>
    <cn>F. Meier</cn>
    <gvOuId>AT:L6:1299</gvOuId>
    <ou>L6AL-F2/c</ou>
    <mail>fmeier@stmk.gv.at</mail>
    <tel>fmeier@stmk.gv.at</tel>
    <gvSecClass>2</gvSecClass>
    <gvGid>AT:B:0:UhO5RG++klaOTsVY+CU=</gvGid>
    <gvFunction>SB</gvFunction>
  </userPrincipal>
  </authenticate>
  <authorize>
    <role value=""ZMR-Fremdenbehoerdenanfrage"">
  <param>
  <key>GKZ</key>
  <value>60100</value>
  </param>
  </role>
  </authorize>
</pvpChainedToken>
";

      authorization.SoapHeaderXmlFragment = authToken;
      SoapFilter filter = new SoapFilter(authorization, 1000, PvpTokenHandling.remove, null, null);
      XmlElement pvpToken = filter.InsertAuthorization(doc);

      Assert.IsNotNull(pvpToken);
      Assert.AreEqual(authToken.InnerXml, pvpToken.InnerXml);

      XmlElement envelope = filter.SelectXmlElement(doc.DocumentElement, "Envelope",
                                                    "http://schemas.xmlsoap.org/soap/envelope/");
      Assert.IsNotNull(envelope);

      Assert.AreEqual(pvpToken.OuterXml, pvpToken.ParentNode.InnerXml);
    }

    [Test]
    public void PvpChainedTokenForSoapWithHeaderElementAndHeaderCollection()
    {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(
        @"<S:Envelope
xmlns:P=""http://egov.gv.at/pvp1.xsd""
xmlns:S=""http://schemas.xmlsoap.org/soap/envelope/""

xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
<S:Header>
</S:Header>
<S:Body>
</S:Body>
</S:Envelope>");

      CustomAuthorization authorization = new CustomAuthorization();
      XmlDocument authDoc = new XmlDocument();
      XmlElement authToken = authDoc.CreateElement("P:pvpToken", "http://egov.gv.at/pvp1.xsd");
      authToken.SetAttribute("version", "1.9");
      authToken.InnerXml =
        @"<authenticate>
<participantId>AT:L6:1234789</participantId>
<systemPrincipal>
<userId>egovstar.appserv1.intra.xyz.gv.at</userId>
<cn>Anwendung 1 Register-Interface</cn>
<gvOuId>AT:L6:4711</gvOuId>
<ou>Fachabteilung 1B Informationstechnik</ou>
<gvOuOKZ>AT:L6-FA1B</gvOuOKZ>
<gvSecClass>2</gvSecClass>
</systemPrincipal>
</authenticate>
<authorize>
<role value=""Registerabfrage""/>
</authorize>";

      authorization.SoapHeaderXmlFragment = authToken;

      NameValueCollection headers = new NameValueCollection()
                                      {
                                        {"X-Version","1.9"},
                                        {"X-AUTHENTICATE-participantId","AT:L6:1234789"},
                                        {"X-AUTHENTICATE-UserId","mmustermann@kommunalnet.at"},
                                        {"X-AUTHENTICATE-cn","Max Mustermann"},
                                        {"X-AUTHENTICATE-gvGid","AT:B:0:LxXnvpcYZesiqVXsZG0bB=="},
                                        {"X-AUTHENTICATE-gvOuId","AT:GGA-60420:0815"},
                                        {"X-AUTHENTICATE-Ou","Meldeamt"},
                                        {"X-AUTHENTICATE-gvOuOKZ","AT:GGA-60420-Abt13"},
                                        {"X-AUTHENTICATE-mail","max.mustermann@hatzendorf.steiermark.at"},
                                        {"X-AUTHENTICATE-tel","+43 3155 5153"},
                                        {"X-AUTHENTICATE-gvSecClass","2"},
                                        {"X-AUTHORIZE-roles","Beispielrolle(GKZ=60420)"},
                                      };

      SoapFilter filter = new SoapFilter(authorization, 1000, PvpTokenHandling.chain, headers, null);
      XmlElement pvpToken = filter.InsertAuthorization(doc);

      Assert.IsNotNull(pvpToken);
      string expectedValue = authToken.InnerXml + @"<pvpChainedToken version=""1.9"" xmlns=""http://egov.gv.at/pvp1.xsd""><authenticate><participantId>AT:L6:1234789</participantId>" + 
        "<userPrincipal>" + "<userId>mmustermann@kommunalnet.at</userId>" + "<cn>Max Mustermann</cn>" + "<gvOuId>AT:GGA-60420:0815</gvOuId>" + "<ou>Meldeamt</ou>" + "<gvOuOKZ>AT:GGA-60420-Abt13</gvOuOKZ>" + 
        "<mail>max.mustermann@hatzendorf.steiermark.at</mail>" + "<tel>+43 3155 5153</tel>" + "<gvSecClass>2</gvSecClass>" + "<gvGid>AT:B:0:LxXnvpcYZesiqVXsZG0bB==</gvGid>" + "</userPrincipal></authenticate><authorize>" + 
        @"<role value=""Beispielrolle""><param><key>GKZ</key><value>60420</value></param></role>" + "</authorize></pvpChainedToken>";
      Assert.IsTrue(pvpToken.InnerXml.StartsWith(authToken.InnerXml + @"<pvpChainedToken version=""1.9"" xmlns=""http://egov.gv.at/pvp1.xsd""><authenticate>"));

      string participant = pvpToken.InnerXml.Substring(pvpToken.InnerXml.IndexOf(@"<pvpChainedToken version=""1.9"" xmlns=""http://egov.gv.at/pvp1.xsd""><authenticate><participantId>")+80);
      Assert.IsTrue(participant.StartsWith("<participantId>AT:L6:1234789</participantId>"));

      string userPrincipal = participant.Substring(participant.IndexOf("<userPrincipal>"));
      Assert.IsTrue(userPrincipal.Contains("<userId>mmustermann@kommunalnet.at</userId>"));
      Assert.IsTrue(userPrincipal.Contains("<cn>Max Mustermann</cn>"));
      Assert.IsTrue(userPrincipal.Contains("<gvOuId>AT:GGA-60420:0815</gvOuId>"));
      Assert.IsTrue(userPrincipal.Contains("<ou>Meldeamt</ou>"));
      Assert.IsTrue(userPrincipal.Contains("<gvOuOKZ>AT:GGA-60420-Abt13</gvOuOKZ>"));
      Assert.IsTrue(userPrincipal.Contains("<mail>max.mustermann@hatzendorf.steiermark.at</mail>"));
      Assert.IsTrue(userPrincipal.Contains("<tel>+43 3155 5153</tel>"));
      Assert.IsTrue(userPrincipal.Contains("<gvSecClass>2</gvSecClass>"));
      Assert.IsTrue(userPrincipal.Contains("<gvGid>AT:B:0:LxXnvpcYZesiqVXsZG0bB==</gvGid>"));

      string roles = userPrincipal.Substring(userPrincipal.IndexOf("</userPrincipal></authenticate><authorize>") + 42);
      Assert.IsTrue(roles.Contains(@"<role value=""Beispielrolle""><param><key>GKZ</key><value>60420</value></param></role>"));

      XmlElement envelope = filter.SelectXmlElement(doc.DocumentElement, "Envelope",
                                                    "http://schemas.xmlsoap.org/soap/envelope/");
      Assert.IsNotNull(envelope);

      Assert.AreEqual(pvpToken.OuterXml, pvpToken.ParentNode.InnerXml);
    }

    [Test]
    public void PvpChainedTokenForSoapWithHeaderElementNoHeaderCollection()
    {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(
        @"<S:Envelope
xmlns:S=""http://schemas.xmlsoap.org/soap/envelope/""

xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
<S:Header>
<wsse:Security>
<pvpToken version=""1.9"" xmlns=""http://egov.gv.at/pvp1.xsd"">
<authenticate>
<participantId>AT:L6:1234789</participantId>
<userPrincipal>
<userId>mmustermann@kommunalnet.at</userId>
<cn>Max Mustermann</cn>
<gvOuId>AT:GGA-60420:0815</gvOuId>
<ou>Meldeamt</ou>
<gvOuOKZ>AT:GGA-60420-Abt13</gvOuOKZ>
<gvSecClass>2</gvSecClass>
<gvGid>AT:B:0:LxXnvpcYZesiqVXsZG0bB==</gvGid>
<mail>max.mustermann@hatzendorf.steiermark.at</mail>
<tel>+43 3155 5153</tel>
</userPrincipal>
</authenticate>
<authorize>
<role value=""Beispielrolle"">
<param>
<key>GKZ</key>
<value>60420</value>
</param>
</role>
</authorize></pvpToken>
</wsse:Security>
</S:Header>
<S:Body>
</S:Body>
</S:Envelope>");

      CustomAuthorization authorization = new CustomAuthorization();
      XmlDocument authDoc = new XmlDocument();
      XmlElement authToken = authDoc.CreateElement("P:pvpToken", "http://egov.gv.at/pvp1.xsd");
      authToken.SetAttribute("version", "1.9");
      authToken.InnerXml =
        @"<authenticate>
<participantId>AT:L6:1234789</participantId>
<systemPrincipal>
<userId>egovstar.appserv1.intra.xyz.gv.at</userId>
<cn>Anwendung 1 Register-Interface</cn>
<gvOuId>AT:L6:4711</gvOuId>
<ou>Fachabteilung 1B Informationstechnik</ou>
<gvOuOKZ>AT:L6-FA1B</gvOuOKZ>
<gvSecClass>2</gvSecClass>
</systemPrincipal>
</authenticate>
<authorize>
<role value=""Registerabfrage"" />
</authorize>";

      authorization.SoapHeaderXmlFragment = authToken;
      authorization.PvpVersion = "1.9";

      SoapFilter filter = new SoapFilter(authorization, 1000, PvpTokenHandling.chain, null, null);
      XmlElement pvpToken = filter.InsertAuthorization(doc);

      Assert.IsNotNull(pvpToken);
      string expectedValue = authToken.InnerXml + @"<pvpChainedToken version=""1.9"" xmlns=""http://egov.gv.at/pvp1.xsd""><authenticate><participantId>AT:L6:1234789</participantId><userPrincipal><userId>mmustermann@kommunalnet.at</userId><cn>Max Mustermann</cn><gvOuId>AT:GGA-60420:0815</gvOuId><ou>Meldeamt</ou><gvOuOKZ>AT:GGA-60420-Abt13</gvOuOKZ><gvSecClass>2</gvSecClass><gvGid>AT:B:0:LxXnvpcYZesiqVXsZG0bB==</gvGid><mail>max.mustermann@hatzendorf.steiermark.at</mail><tel>+43 3155 5153</tel></userPrincipal></authenticate></pvpChainedToken>";
      Assert.AreEqual(expectedValue, pvpToken.InnerXml);

      XmlElement envelope = filter.SelectXmlElement(doc.DocumentElement, "Envelope",
                                                    "http://schemas.xmlsoap.org/soap/envelope/");
      Assert.IsNotNull(envelope);

      Assert.AreEqual(pvpToken.OuterXml, pvpToken.ParentNode.InnerXml);
    }

    [Test]
    public void PvpChainedTokenForSoapWithoutHeaderElementNoHeaderCollection()
    {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(
        @"<S:Envelope
xmlns:S=""http://schemas.xmlsoap.org/soap/envelope/""

xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
<S:Body>
</S:Body>
</S:Envelope>");

      CustomAuthorization authorization = new CustomAuthorization();
      XmlDocument authDoc = new XmlDocument();
      XmlElement authToken = authDoc.CreateElement("P:pvpToken", "http://egov.gv.at/pvp1.xsd");
      authToken.SetAttribute("version", "1.9");
      authToken.InnerXml =
        @"<authenticate>
<participantId>AT:L6:1234789</participantId>
<systemPrincipal>
<userId>egovstar.appserv1.intra.xyz.gv.at</userId>
<cn>Anwendung 1 Register-Interface</cn>
<gvOuId>AT:L6:4711</gvOuId>
<ou>Fachabteilung 1B Informationstechnik</ou>
<gvOuOKZ>AT:L6-FA1B</gvOuOKZ>
<gvSecClass>2</gvSecClass>
</systemPrincipal>
</authenticate>
<authorize>
<role value=""Registerabfrage"" />
</authorize>";

      authorization.SoapHeaderXmlFragment = authToken;

      SoapFilter filter = new SoapFilter(authorization, 1000, PvpTokenHandling.chain, null, null);
      XmlElement pvpToken = filter.InsertAuthorization(doc);

      Assert.IsNotNull(pvpToken);
      string expectedValue = authToken.InnerXml;
      Assert.AreEqual(expectedValue, pvpToken.InnerXml);

      XmlElement envelope = filter.SelectXmlElement(doc.DocumentElement, "Envelope",
                                                    "http://schemas.xmlsoap.org/soap/envelope/");
      Assert.IsNotNull(envelope);

      Assert.AreEqual(pvpToken.OuterXml, pvpToken.ParentNode.InnerXml);
    }

    private const string EnvStart =
      @"<S:Envelope
xmlns:P=""http://egov.gv.at/pvp1.xsd""
xmlns:S=""http://schemas.xmlsoap.org/soap/envelope/""
xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""
xmlns:seleg=""http://schemas.xmlsoap.org/ws/2002/04/secext"">";

    private static XmlDocument GetSoapWithHeaderAndSecurity()
    {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(EnvStart +
                  @"<S:Header>
<wsse:Security>
</wsse:Security>
</S:Header>
<S:Body>
</S:Body>
</S:Envelope>");
      return doc;
    }

    private static XmlDocument GetSoapBodyOnly()
    {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(EnvStart +
                  @"<S:Body>
</S:Body>
</S:Envelope>");
      return doc;
    }

    private static XmlDocument GetSoapWithPvpToken()
    {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(EnvStart +
                  @"<S:Header>
<wsse:Security>
<P:pvpToken version=""1.9"">
  <authenticate>
    <participantId>dfsfsd</participantId>
    <userPrincipal>
      <userId></userId>
      <cn></cn>
      <gvOuId></gvOuId>
      <ou></ou>
      <gvGid></gvGid>
    </userPrincipal>
  </authenticate>
  <authorize>
  </authorize>
</P:pvpToken>
</wsse:Security>
</S:Header>
<S:Body>
</S:Body>
</S:Envelope>");
      return doc;
    }

    private static XmlElement GetAuthToken19(XmlDocument authDoc)
    {
      XmlElement authToken = authDoc.CreateElement("P:pvpToken", "http://egov.gv.at/pvp1.xsd");
      authToken.SetAttribute("version", "1.9");
      authToken.InnerXml =
        @"<authenticate>
<participantId>AT:L6:994</participantId>
<userPrincipal>
<userId>fmeier@stmk.gv.at</userId>
<cn>F. Meier</cn>
<gvOuId>AT:L6:1299</gvOuId>
<ou>L6AL-F2/c</ou>
<mail>fmeier@stmk.gv.at</mail>
<tel>fmeier@stmk.gv.at</tel>
<gvSecClass>2</gvSecClass>
<gvGid>AT:B:0:UhO5RG++klaOTsVY+CU=</gvGid>
<gvFunction>SB</gvFunction>
</userPrincipal>
</authenticate>
<authorize>
<role value=""ZMR-Fremdenbehoerdenanfrage"">
<param>
<key>GKZ</key>
<value>60100</value>
</param>
</role>
</authorize>";
      return authToken;
    }

    private static XmlElement GetAuthToken18(XmlDocument authDoc)
    {
      XmlElement authToken = authDoc.CreateElement("P:pvpToken", "http://egov.gv.at/pvp1.xsd");
      authToken.SetAttribute("version", "1.8");
      authToken.InnerXml =
        @"<authenticate>
<participantId>AT:L6:994</participantId>
<userPrincipal>
<userId>fmeier@stmk.gv.at</userId>
<cn>F. Meier</cn>
<gvOuId>AT:L6:1299</gvOuId>
<ou>L6AL-F2/c</ou>
<mail>fmeier@stmk.gv.at</mail>
<tel>fmeier@stmk.gv.at</tel>
<gvSecClass>2</gvSecClass>
<gvGid>AT:B:0:UhO5RG++klaOTsVY+CU=</gvGid>
<gvFunction>SB</gvFunction>
</userPrincipal>
</authenticate>
<authorize>
<role value=""ZMR-Fremdenbehoerdenanfrage"">
<param>
<key>GKZ</key>
<value>60100</value>
</param>
</role>
</authorize>";
      return authToken;
    }

  }
}