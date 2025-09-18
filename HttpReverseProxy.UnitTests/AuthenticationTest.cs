/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Egora.Stammportal.HttpReverseProxy.CustomAuthentication;
using Egora.Stammportal.HttpReverseProxy.Mapping;
using Egora.Stammportal.HttpReverseProxy.UnitTests.Properties;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests
{
  //[TestFixture]
  public class AuthenticationTest
  {
    [Test]
    public void UriTest()
    {
      var uri = new Uri("https://server/dir1/dir2/file.ext?name1=valuewith%2F");
      var redirect = uri.PathAndQuery;
      Assert.That(System.Web.HttpUtility.UrlEncode(redirect), Is.EqualTo("%2fdir1%2fdir2%2ffile.ext%3fname1%3dvaluewith%252F"));
    }

    [Test]
    public void ReadCertificateMapping()
    {
      var testModule = new TestCertificateAuthenticationModule();
      testModule.TestInit();
      var config = testModule.Configuration;
      Assert.That(config, Is.Not.Null);
      Assert.That(config.HeaderName, Is.EqualTo("X-Forwarded-ClientCert"));
      Assert.That(config.Mappings, Is.Not.Null);
      Assert.That(config.Mappings.Length, Is.EqualTo(2));
    }
    [Test]
    public void GetUsernameTest()
    {
      var testModule = new TestCertificateAuthenticationModule();
      testModule.TestInit();
      var config = testModule.Configuration;
      Assert.That(config, Is.Not.Null);
      Assert.That(config.Mappings.Length, Is.EqualTo(2));
      var cert = new X509Certificate2(File.ReadAllBytes("rubicon.eu.crt"));
      var (userName, isAdmin, secClass) = config.GetUserInfo(null, cert.Subject, cert.Issuer);
      Assert.That(userName, Is.EqualTo("DerUser"));
      Assert.That(isAdmin, Is.False);

      (userName, isAdmin, secClass) = config.GetUserInfo(cert.Thumbprint, null, null);
      Assert.That(userName, Is.EqualTo("DerAdmin"));
      Assert.That(isAdmin, Is.True);
    }
    [Test]
    public void EvaluateUsernameTest()
    {
      var testModule = new TestCertificateAuthenticationModule();
      testModule.TestInit();
      NameValueCollection header = new NameValueCollection();
      header.Add("X_UserName", "Ein KiGa User");
      var userName = testModule.EvalUser(header, "$[HTTP_X_UserName]");
      Assert.That(userName, Is.EqualTo("Ein KiGa User"));
      var userName2 = testModule.EvalUser(header, "Ein KiGa User");
      Assert.That(userName2, Is.EqualTo("Ein KiGa User"));
    }
  }

  public class TestCertificateAuthenticationModule : CertificateAuthenticationModule
  {
    public void TestInit()
    {
      base.ReadConfiguration("CertificateUserMapping.xml");
    }

    public CertificateAuthenticationConfiguration Configuration => Config;

    public string EvalUser(NameValueCollection headers, string userName)
    {
      return base.EvaluateUserName(headers, userName);
    }
  }
}