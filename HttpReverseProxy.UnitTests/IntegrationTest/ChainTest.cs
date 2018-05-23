/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests.IntegrationTest
{
  [TestFixture]
  public class ChainTest
  {
    private Stream responseStream;

    [Test]
    public void WebAppChainingTest19()
    {
      HttpWebRequest request = CreateRequest1();
      request.Headers.Add("X-Version", "1.9");
      request.Headers.Add("X-AUTHENTICATE-participantId", "UnitTest");
      request.Headers.Add("X-AUTHENTICATE-UserID", "WebAppChainingTest");

      Exception e = null;
      HttpWebResponse response = GetResponse(request, out e);
      Assert.IsNotNull(response, "Response");

      responseStream = response.GetResponseStream();
      string errorPage;
      if (e != null)
        errorPage = new StreamReader(responseStream).ReadToEnd();
      Assert.IsNull(e);
      XmlSerializer serializer = new XmlSerializer(typeof(RequestInformation));
      RequestInformation info = (RequestInformation)serializer.Deserialize(responseStream);

      Assert.IsNotNull(info, "RequestInformation");
      Assert.AreEqual("1.9", info.GetHeader("X-01-Version"));

      Assert.IsNotNull(info.GetHeader("X-AUTHENTICATE-cn"), "X-AUTHENTICATE-cn");
      Assert.AreEqual("common name", info.GetHeader("X-AUTHENTICATE-cn"));

      Assert.IsNotNull(info.GetHeader("X-01-AUTHENTICATE-participantId"), "X-01-AUTHENTICATE-participantId");
      Assert.AreEqual("UnitTest", info.GetHeader("X-01-AUTHENTICATE-participantId"));

      Assert.IsNotNull(info.GetHeader("X-01-AUTHENTICATE-UserID"), "X-01-AUTHENTICATE-UserID");
      Assert.AreEqual("WebAppChainingTest", info.GetHeader("X-01-AUTHENTICATE-UserID"));
    }

    [Test]
    public void WebAppChainingTest21()
    {
      HttpWebRequest request = CreateRequest2();
      request.Headers.Add("X-PVP-Version", "2.1");
      request.Headers.Add("X-PVP-participantId", "UnitTest");
      request.Headers.Add("X-PVP-UserID", "WebAppChainingTest");

      Exception e = null;
      HttpWebResponse response = GetResponse(request, out e);
      Assert.IsNotNull(response, "Response");

      responseStream = response.GetResponseStream();
      string errorPage;
      if (e != null)
        errorPage = new StreamReader(responseStream).ReadToEnd();
      Assert.IsNull(e);
      XmlSerializer serializer = new XmlSerializer(typeof(RequestInformation));
      RequestInformation info = (RequestInformation)serializer.Deserialize(responseStream);

      Assert.IsNotNull(info, "RequestInformation");

      Assert.IsNotNull(info.GetHeader("X-PVP-PRINCIPAL-NAME"), "X-PVP-PRINCIPAL-NAME");
      Assert.AreEqual("common name", info.GetHeader("X-PVP-PRINCIPAL-NAME"));

      Assert.IsNotNull(info.GetHeader("X-PVP-participantId_01"), "X-PVP-participantId_01");
      Assert.AreEqual("UnitTest", info.GetHeader("X-PVP-participantId_01"));

      Assert.IsNotNull(info.GetHeader("X-PVP-UserID_01"), "X-PVP-UserID_01");
      Assert.AreEqual("WebAppChainingTest", info.GetHeader("X-PVP-UserID_01"));
    }

    private HttpWebResponse GetResponse(HttpWebRequest request, out Exception exception)
    {
      exception = null;
      try
      {
        return (HttpWebResponse)request.GetResponse();
      }
      catch (WebException e)
      {
        exception = e;
        return (HttpWebResponse) e.Response;
      }
    }

    [Test]
    public void Web2SoapChainTest()
    {
      HttpWebRequest request =
        (HttpWebRequest)HttpWebRequest.Create("http://egoratest/Stammportal/localsoapchain1/TestService.asmx ");
      request.Headers.Add("SOAPAction", @"""http://tempuri.org/DoSomeThing""");
      request.Headers.Add("X-Version", "1.9");
      request.Headers.Add("X-AUTHENTICATE-participantId", "UnitTest");
      request.Headers.Add("X-AUTHENTICATE-UserID", "SoapChainingTest");
      request.ContentType = "text/xml; charset=utf-8";
      request.Method = "POST";

      string message =
        @"<?xml version='1.0' encoding='utf-8'?>
<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
  <soap:Header>
    <Security xmlns='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'></Security>
  </soap:Header>
  <soap:Body>
    <DoSomeThing xmlns='http://tempuri.org/' />
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

      byte[] responseContent = new byte[response.ContentLength];
      responseStream.Read(responseContent, 0, responseContent.Length);
      string responseText = Encoding.UTF8.GetString(responseContent);
      Assert.IsTrue(responseText.Contains("pvpChainedToken"));
      Assert.IsTrue(responseText.Contains("SoapChainingTest"));
    }

    [Test]
    public void Soap2SoapChainTest()
    {
      HttpWebRequest request =
        (HttpWebRequest)HttpWebRequest.Create("http://egoratest/Stammportal/localsoapchain1/TestService.asmx ");
      request.Headers.Add("SOAPAction", @"""http://tempuri.org/DoSomeThing""");
      request.ContentType = "text/xml; charset=utf-8";
      request.Method = "POST";

      string message =
        @"<?xml version='1.0' encoding='utf-8'?>
<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
  <soap:Header>
    <Security xmlns='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'>
        <pvpToken version=""1.9"" xmlns=""http://egov.gv.at/pvp1.xsd"">
          <authenticate>
            <participantId>UnitTest</participantId>
            <userPrincipal>
            <userId>Soap2SoapChainTest</userId>
            <cn>commonname</cn>
            <gvSecClass>2</gvSecClass>
            <gvGid>EEC32184-398E-4C61-82E9-3D6F767F142D</gvGid>
            <gvFunction>TESTER</gvFunction>
            </userPrincipal>
          </authenticate>
          <authorize>
            <role value=""UT-Test"">
              <param>
                <key>test</key>
                <value>soap</value>
              </param>
            </role>
          </authorize>
         </pvpToken>
      </Security>
    </soap:Header>
  <soap:Body>
    <DoSomeThing xmlns='http://tempuri.org/' />
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

      byte[] responseContent = new byte[response.ContentLength];
      responseStream.Read(responseContent, 0, responseContent.Length);
      string responseText = Encoding.UTF8.GetString(responseContent);
      Assert.IsTrue(responseText.Contains("pvpChainedToken"));
      Assert.IsTrue(responseText.Contains("EEC32184-398E-4C61-82E9-3D6F767F142D"));
    }

    private HttpWebRequest CreateRequest1()
    {
      HttpWebRequest request =
        (HttpWebRequest)WebRequest.Create("http://egoratest/Stammportal/localtestchain1/IntegrationTestPage.aspx");
      request.UseDefaultCredentials = true;
      return request;
    }

    private HttpWebRequest CreateRequest2()
    {
      HttpWebRequest request =
        (HttpWebRequest)WebRequest.Create("http://egoratest/Stammportal/localtestchain2/IntegrationTestPage.aspx");
      request.UseDefaultCredentials = true;
      return request;
    }
  }
}