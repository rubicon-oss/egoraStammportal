/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests.IntegrationTest
{
  [TestFixture]
  public class SoapTest
  {
    [Test]
    public void SoapHeader()
    {
      TestService service = new TestService();
      service.UseDefaultCredentials = true;
      string result = service.DoSomeThing();
      Assert.IsNotNull(result, "result");
      XmlDocument resultToken = new XmlDocument();
      resultToken.LoadXml(result);
      XmlDocument pvpToken = new XmlDocument();
      pvpToken.LoadXml(
        @"<pvpToken version=""1.9"" xmlns=""http://egov.gv.at/pvp1.xsd"">
<authenticate>
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
</authorize>
</pvpToken>");

      Assert.AreEqual(pvpToken.OuterXml, resultToken.OuterXml);
    }

    [Test]
    public void SoapHeader2()
    {
      TestService service = new TestService();
      service.Url = "http://egoratest/Stammportal/localsoap2/TestService.asmx";
      service.UseDefaultCredentials = true;
      string result = service.DoSomeThing();
      Assert.IsNotNull(result, "result");
      XmlDocument resultToken = new XmlDocument();
      resultToken.LoadXml(result);
      XmlDocument pvpToken = new XmlDocument();
      pvpToken.LoadXml(
@"<AttributeStatement ID=""_b40a1601-5376-4bed-9544-4d4ad024d0f6"" Version=""2.0"" xmlns=""urn:oasis:names:tc:SAML:2.0:assertion"">
<Attribute Name=""urn:oid:1.2.40.0.10.2.1.1.261.10"" FriendlyName=""PVP-VERSION"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:uri""><AttributeValue>2.1</AttributeValue></Attribute>
<Attribute Name=""urn:oid:1.2.40.0.10.2.1.1.71"" FriendlyName=""PARTICIPANT-ID"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:uri""><AttributeValue>AT:L6:994</AttributeValue></Attribute>
<Attribute Name=""urn:oid:0.9.2342.19200300.100.1.1"" FriendlyName=""USERID"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:uri""><AttributeValue>fmeier@stmk.gv.at</AttributeValue></Attribute>
<Attribute Name=""urn:oid:1.2.40.0.10.2.1.1.261.20"" FriendlyName=""PRINCIPAL-NAME"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:uri""><AttributeValue>Meier</AttributeValue></Attribute>
<Attribute Name=""urn:oid:2.5.4.42"" FriendlyName=""GIVEN-NAME"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:uri""><AttributeValue>Franz</AttributeValue></Attribute>
<Attribute Name=""urn:oid:0.9.2342.19200300.100.1.3"" FriendlyName=""MAIL"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:uri""><AttributeValue>fmeier@stmk.gv.at</AttributeValue></Attribute>
<Attribute Name=""urn:oid:1.2.40.0.10.2.1.1.1"" FriendlyName=""GID"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:uri""><AttributeValue>AT:B:0:UhO5RG++klaOTsVY+CU=</AttributeValue></Attribute>
<Attribute Name=""urn:oid:1.2.40.0.10.2.1.1.3"" FriendlyName=""OU-GV-OU-ID"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:uri""><AttributeValue>AT:L6:1299</AttributeValue></Attribute>
<Attribute Name=""urn:oid:2.5.4.11"" FriendlyName=""OU"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:uri""><AttributeValue>L6AL-F2/c</AttributeValue></Attribute>
<Attribute Name=""http://lfrz.at/stdportal/names/pvp/secClass"" FriendlyName=""SECCLASS"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:uri""><AttributeValue>2</AttributeValue></Attribute>
<Attribute Name=""urn:oid:1.2.40.0.10.2.1.1.33"" FriendlyName=""FUNCTION"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:uri""><AttributeValue>SB</AttributeValue></Attribute>
<Attribute Name=""urn:oid:1.2.40.0.10.2.1.1.261.30"" FriendlyName=""ROLES"" NameFormat=""urn:oasis:names:tc:SAML:2.0:attrname-format:uri""><AttributeValue>ZMR-Fremdenbehoerdenanfrage(GKZ=60100)</AttributeValue></Attribute>
</AttributeStatement>");

      string expected = pvpToken.OuterXml;
      string actual = resultToken.OuterXml;
      Assert.IsTrue(actual.StartsWith("<AttributeStatement ID="));
      Assert.IsTrue(actual.EndsWith(expected.Substring(expected.IndexOf(@" Version=""2.0"""))));
    }

    [Test]
    public void SoapErrorDirect()
    {
      HttpWebRequest request =
        (HttpWebRequest)HttpWebRequest.Create("http://egoratest/PvpTestService/1/TestService.asmx ");
      request.Headers.Add("SOAPAction", @"""http://tempuri.org/DoError""");
      request.ContentType = "text/xml; charset=utf-8";
      request.Method = "POST";

      string message =
        @"<?xml version='1.0' encoding='utf-8'?>
<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
  <soap:Header>
    <Security xmlns='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'></Security>
  </soap:Header>
  <soap:Body>
    <DoError xmlns='http://tempuri.org/' />
  </soap:Body>
</soap:Envelope>
";
      byte[] buffer = Encoding.UTF8.GetBytes(message);
      request.ContentLength = buffer.Length;
      WebResponse response = null;
      using (WindowsImpersonationContext impersonationContext = WindowsIdentity.GetCurrent().Impersonate())
      {
        request.Credentials = CredentialCache.DefaultCredentials;
        request.GetRequestStream().Write(buffer, 0, buffer.Length);
        try
        {
          response = request.GetResponse();
        }
        catch (WebException ex)
        {
          response = ex.Response;
        }
        impersonationContext.Undo();
      }

      Assert.IsNotNull(response);
      Stream responseStream = response.GetResponseStream();
      Assert.IsNotNull(responseStream);

      byte[] responseContent = new byte[responseStream.Length];
      responseStream.Read(responseContent, 0, responseContent.Length);
      string responseText = Encoding.UTF8.GetString(responseContent);
      Assert.IsTrue(
        responseText.ToLowerInvariant().StartsWith(
          @"<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><soap:Fault><faultcode>soap:Server</faultcode><faultstring>System.Web.Services.Protocols.SoapException: Server was unable to process request. ---&gt; System.ApplicationException: The error occured.
   at TestPvpService.TestService.DoError() in ".ToLowerInvariant()));
      Assert.IsTrue(
        responseText.ToLowerInvariant().EndsWith(
          @"   --- End of inner exception stack trace ---</faultstring><detail /></soap:Fault></soap:Body></soap:Envelope>".ToLowerInvariant()));
    }

    [Test]
    public void SoapErrorViaPortal()
    {
      HttpWebRequest request =
        (HttpWebRequest)HttpWebRequest.Create("http://egoratest/Stammportal/localsoap1/TestService.asmx ");
      request.Headers.Add("SOAPAction", @"""http://tempuri.org/DoError""");
      request.ContentType = "text/xml; charset=utf-8";
      request.Method = "POST";

      string message =
        @"<?xml version='1.0' encoding='utf-8'?>
<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
  <soap:Header>
    <Security xmlns='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'></Security>
  </soap:Header>
  <soap:Body>
    <DoError xmlns='http://tempuri.org/' />
  </soap:Body>
</soap:Envelope>
";
      byte[] buffer = Encoding.UTF8.GetBytes(message);
      request.ContentLength = buffer.Length;
      WebResponse response = null;
      using (WindowsImpersonationContext impersonationContext = WindowsIdentity.GetCurrent().Impersonate())
      {
        request.Credentials = CredentialCache.DefaultCredentials;
        request.GetRequestStream().Write(buffer, 0, buffer.Length);
        try
        {
          response = request.GetResponse();
        }
        catch (WebException ex)
        {
          response = ex.Response;
        }
        impersonationContext.Undo();
      }

      Assert.IsNotNull(response);
      Stream responseStream = response.GetResponseStream();
      Assert.IsNotNull(responseStream);

      byte[] responseContent = new byte[responseStream.Length];
      responseStream.Read(responseContent, 0, responseContent.Length);
      string responseText = Encoding.UTF8.GetString(responseContent);
      Assert.IsTrue(responseText.ToLowerInvariant().StartsWith(
        @"<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><soap:Fault><faultcode>soap:Server</faultcode><faultstring>System.Web.Services.Protocols.SoapException: Server was unable to process request. ---&gt; System.ApplicationException: The error occured.
   at TestPvpService.TestService.DoError()".ToLowerInvariant()));
      Assert.IsTrue( responseText.ToLowerInvariant().EndsWith( 
      @"  --- End of inner exception stack trace ---</faultstring><detail /></soap:Fault></soap:Body></soap:Envelope>".ToLowerInvariant()));
    }
  }

  [System.ComponentModel.DesignerCategory("code")]
  [System.Web.Services.WebServiceBinding(Name = "TestServiceSoap", Namespace = "http://tempuri.org/")]
  public partial class TestService : System.Web.Services.Protocols.SoapHttpClientProtocol
  {
    private Security securityValueField;

    private bool useDefaultCredentialsSetExplicitly;

    public TestService()
    {
      this.Url = "http://egoratest/Stammportal/localsoap1/TestService.asmx";
      this.useDefaultCredentialsSetExplicitly = true;
    }

    public Security SecurityValue
    {
      get { return this.securityValueField; }
      set { this.securityValueField = value; }
    }

    public new string Url
    {
      get { return base.Url; }
      set
      {
        if (this.useDefaultCredentialsSetExplicitly == false)
        {
          base.UseDefaultCredentials = false;
        }
        base.Url = value;
      }
    }

    public new bool UseDefaultCredentials
    {
      get { return base.UseDefaultCredentials; }
      set
      {
        base.UseDefaultCredentials = value;
        this.useDefaultCredentialsSetExplicitly = true;
      }
    }


    /// <remarks/>
    [System.Web.Services.Protocols.SoapHeader("SecurityValue")]
    [System.Web.Services.Protocols.SoapDocumentMethod("http://tempuri.org/DoSomeThing",
      RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/",
      Use = System.Web.Services.Description.SoapBindingUse.Literal,
      ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public string DoSomeThing()
    {
      object[] results = this.Invoke("DoSomeThing", new object[0]);
      return ((string) (results[0]));
    }
  }

  /// <remarks/>
  [System.Serializable()]
  [System.ComponentModel.DesignerCategory("code")]
  [System.Xml.Serialization.XmlType(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
  [System.Xml.Serialization.XmlRoot(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", IsNullable = false)]
  public partial class Security : System.Web.Services.Protocols.SoapHeader
  {
    private System.Xml.XmlElement anyField;

    private System.Xml.XmlAttribute[] anyAttrField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAnyElement()]
    public System.Xml.XmlElement Any
    {
      get { return this.anyField; }
      set { this.anyField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAnyAttribute()]
    public System.Xml.XmlAttribute[] AnyAttr
    {
      get { return this.anyAttrField; }
      set { this.anyAttrField = value; }
    }
  }
}