/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests.IntegrationTest
{
  [TestFixture]
  public class CertificateTest
  {
    [Test]
    public void NoCert()
    {
      HttpWebRequest request = CreateRequest();
      HttpWebResponse response;
      try
      {
        response = (HttpWebResponse)request.GetResponse();
      }
      catch (WebException webException)
      {
        response = (HttpWebResponse)webException.Response;
      }
      Assert.IsNotNull(response, "Response");
      Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
      Assert.That(response.StatusDescription, Is.EqualTo("No Cert found."));
    }

    [Test]
    public void CertOK()
    {
      var testModule = new TestCertificateAuthenticationModule();
      testModule.TestInit();
      var config = testModule.Configuration;
      HttpWebRequest request = CreateRequest();
      var cert = new X509Certificate2(File.ReadAllBytes("rubicon.eu.crt"));
      Assert.That(cert.Verify(), Is.True);

      var binaryCert = cert.Export(X509ContentType.Cert);
      request.Headers.Add(config.HeaderName, Convert.ToBase64String(binaryCert));
      request.CookieContainer = new CookieContainer();
      HttpWebResponse response;
      try
      {
        response = (HttpWebResponse)request.GetResponse();
      }
      catch (WebException webException)
      {
        response = (HttpWebResponse)webException.Response;
        Assert.Fail($"Status={response.StatusCode}");
      }
      Assert.IsNotNull(response, "Response");
      Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
      Assert.That(response.Cookies.Count, Is.EqualTo(1));
      var cookie = response.Cookies[0];
      Assert.That(cookie.Name, Is.EqualTo("CookieForCertMapping"));
    }

    [Test]
    public void CertOKAndSendAgainWithoutCert()
    {
      var testModule = new TestCertificateAuthenticationModule();
      testModule.TestInit();
      var config = testModule.Configuration;
      HttpWebRequest request = CreateRequest();
      var cert = new X509Certificate2(File.ReadAllBytes("rubicon.eu.crt"));

      var binaryCert = cert.Export(X509ContentType.Cert);
      request.Headers.Add(config.HeaderName, Convert.ToBase64String(binaryCert));
      var cookieContainer = new CookieContainer();
      request.CookieContainer = cookieContainer;
      HttpWebResponse response;
      try
      {
        response = (HttpWebResponse)request.GetResponse();
      }
      catch (WebException webException)
      {
        response = (HttpWebResponse)webException.Response;
        Assert.Fail($"Status={response.StatusCode}");
      }
      Assert.IsNotNull(response, "Response");
      Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
      Assert.That(response.Cookies.Count, Is.EqualTo(1));
      var cookie = response.Cookies[0];
      Assert.That(cookie.Name, Is.EqualTo("CookieForCertMapping"));

      HttpWebRequest request2 = CreateRequest();
      request2.CookieContainer = cookieContainer;
      HttpWebResponse response2;
      try
      {
        response2 = (HttpWebResponse)request.GetResponse();
      }
      catch (WebException webException)
      {
        response2 = (HttpWebResponse)webException.Response;
        Assert.Fail($"Status={response2.StatusCode}");
      }
      Assert.IsNotNull(response2, "response2");
      Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
      Assert.That(response2.Cookies.Count, Is.EqualTo(1));
      cookie = response2.Cookies[0];
      Assert.That(cookie.Name, Is.EqualTo("CookieForCertMapping"));
    }
    private HttpWebRequest CreateRequest(string query)
    {
      HttpWebRequest request =
        (HttpWebRequest)WebRequest.Create("http://egoratest/stammportal/certificatebased/IntegrationTestPage.aspx" + (String.IsNullOrEmpty(query) ? "" : "?"+query));
      request.UseDefaultCredentials = true;
      return request;
    }
    private HttpWebRequest CreateRequest()
    {
      return CreateRequest(null);
    }
  }
}