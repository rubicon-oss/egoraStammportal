/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;
using System.Net;
using System.Web;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests
{
  [TestFixture]
  public class CookieTransformerTest
  {
    [Test]
    public void LeftSideCookie_FromRightSideCookieWithNoDomainAndNoPath()
    {
      CookieTransformer transformer = new CookieTransformer(true, "https://somehost/somepath/");
      Cookie rightSideCookie = new Cookie("TestName", "TestValue");
      DateTime inOneHour = DateTime.Now + new TimeSpan(1, 0, 0);
      rightSideCookie.Expires = inOneHour;
      rightSideCookie.Secure = true;
      rightSideCookie.HttpOnly = true;

      HttpCookie leftSideCookie = transformer.CreateLeftSideResponseCookie(rightSideCookie);

      Assert.IsNotNull(leftSideCookie);
      Assert.AreEqual("somehost/somepath/TestName", leftSideCookie.Name);
      Assert.AreEqual(CookieTransformer.c_CookieSignature + "|||TestValue", leftSideCookie.Value);
      Assert.AreEqual("/", leftSideCookie.Path);
      Assert.AreEqual(inOneHour, leftSideCookie.Expires);
      Assert.IsNull(leftSideCookie.Domain);
      Assert.IsTrue(leftSideCookie.Secure, "Secure");
      Assert.IsTrue(leftSideCookie.HttpOnly, "HttpOnly");
    }

    [Test]
    public void LeftSideCookie_FromRightSideCookieWithSpecialCharactersNoDomainAndNoPath()
    {
      CookieTransformer transformer = new CookieTransformer(true, "https://somehost/somepath/");
      Cookie rightSideCookie = new Cookie("_pk_id.1.4515", "5d53052c68f46bd6.1461831752.1.1461831752.1461831752.");
      DateTime inOneHour = DateTime.Now + new TimeSpan(1, 0, 0);
      rightSideCookie.Expires = inOneHour;
      rightSideCookie.Secure = true;
      rightSideCookie.HttpOnly = true;

      HttpCookie leftSideCookie = transformer.CreateLeftSideResponseCookie(rightSideCookie);

      Assert.IsNotNull(leftSideCookie);
      Assert.AreEqual("somehost/somepath/_pk_id.1.4515", leftSideCookie.Name);
      Assert.AreEqual(CookieTransformer.c_CookieSignature + "|||5d53052c68f46bd6.1461831752.1.1461831752.1461831752.", leftSideCookie.Value);
      Assert.AreEqual("/", leftSideCookie.Path);
      Assert.AreEqual(inOneHour, leftSideCookie.Expires);
      Assert.IsNull(leftSideCookie.Domain);
      Assert.IsTrue(leftSideCookie.Secure, "Secure");
      Assert.IsTrue(leftSideCookie.HttpOnly, "HttpOnly");
    }

    [Test]
    public void LeftSideCookie_FromRightSideCookieWithDomainAndPath()
    {
      CookieTransformer transformer = new CookieTransformer(true, "https://somehost:8443/somepath/");
      Cookie rightSideCookie = new Cookie("TestName", "TestValue");
      DateTime inOneHour = DateTime.Now + new TimeSpan(1, 0, 0);
      rightSideCookie.Domain = "www.domain.com";
      rightSideCookie.Path = "/application";
      rightSideCookie.Expires = inOneHour;
      rightSideCookie.Secure = false;
      rightSideCookie.HttpOnly = false;

      HttpCookie leftSideCookie = transformer.CreateLeftSideResponseCookie(rightSideCookie);

      Assert.IsNotNull(leftSideCookie);
      Assert.AreEqual("somehost:8443/somepath/TestName", leftSideCookie.Name);
      Assert.AreEqual(CookieTransformer.c_CookieSignature + "|www.domain.com|/application|TestValue",
                      leftSideCookie.Value);
      Assert.AreEqual("/", leftSideCookie.Path);
      Assert.AreEqual(inOneHour, leftSideCookie.Expires);
      Assert.IsNull(leftSideCookie.Domain);
      Assert.IsFalse(leftSideCookie.Secure, "Secure");
      Assert.IsFalse(leftSideCookie.HttpOnly, "HttpOnly");
    }

    [Test]
    public void LeftSideCookie_FromRightSideCookieWithDomainAndPath_NoIsolation()
    {
      CookieTransformer transformer = new CookieTransformer(false, "https://somehost/somepath/");
      Cookie rightSideCookie = new Cookie("TestName", "TestValue");
      DateTime inOneHour = DateTime.Now + new TimeSpan(1, 0, 0);
      rightSideCookie.Domain = "www.domain.com";
      rightSideCookie.Path = "/application";
      rightSideCookie.Expires = inOneHour;
      rightSideCookie.Secure = false;
      rightSideCookie.HttpOnly = false;

      HttpCookie leftSideCookie = transformer.CreateLeftSideResponseCookie(rightSideCookie);

      Assert.IsNotNull(leftSideCookie);
      Assert.AreEqual("somehost/TestName", leftSideCookie.Name);
      Assert.AreEqual(CookieTransformer.c_CookieSignature + "|www.domain.com|/application|TestValue",
                      leftSideCookie.Value);
      Assert.AreEqual("/", leftSideCookie.Path);
      Assert.AreEqual(inOneHour, leftSideCookie.Expires);
      Assert.IsNull(leftSideCookie.Domain);
      Assert.IsFalse(leftSideCookie.Secure, "Secure");
      Assert.IsFalse(leftSideCookie.HttpOnly, "HttpOnly");
    }

    [Test]
    public void RightSideCookie_FromLeftSideCookieWithDomainAndPath()
    {
      CookieTransformer transformer = new CookieTransformer(true, "https://somehost:8443/somepath/");
      HttpCookie leftSideCookie = new HttpCookie("somehost:8443/somepath/TestName",
                                                 CookieTransformer.c_CookieSignature +
                                                 "|www.portal.gv.at|/application|TestValue|anotherValue");
      DateTime inOneHour = DateTime.Now + new TimeSpan(1, 0, 0);
      leftSideCookie.Domain = "www.domain.com";
      leftSideCookie.Path = "/somepath";
      leftSideCookie.Expires = inOneHour;
      leftSideCookie.Secure = true;
      leftSideCookie.HttpOnly = true;

      Cookie rightSideCookie = transformer.CreateRightSideRequestCookie(leftSideCookie, "/somepath", true);

      Assert.IsNotNull(rightSideCookie, "Cookie is null");
      Assert.AreEqual("TestName", rightSideCookie.Name);
      Assert.AreEqual("TestValue|anotherValue", rightSideCookie.Value);
      Assert.AreEqual("/application", rightSideCookie.Path);
      Assert.AreEqual(inOneHour, rightSideCookie.Expires);
      Assert.AreEqual("www.portal.gv.at", rightSideCookie.Domain);
      Assert.IsTrue(rightSideCookie.Secure, "Secure");
      Assert.IsTrue(rightSideCookie.HttpOnly, "HttpOnly");
    }

    [Test]
    public void RightSideCookie_FromLeftSideCookieWithDomainAndPathNoIsolation()
    {
        CookieTransformer transformer = new CookieTransformer(false, "https://somehost:8443/somepath/");
        HttpCookie leftSideCookie = new HttpCookie("somehost:8443/TestName",
                                                    CookieTransformer.c_CookieSignature +
                                                    "|www.portal.gv.at|/application|TestValue|anotherValue");
        DateTime inOneHour = DateTime.Now + new TimeSpan(1, 0, 0);
        leftSideCookie.Domain = "www.domain.com";
        leftSideCookie.Path = "/somepath";
        leftSideCookie.Expires = inOneHour;
        leftSideCookie.Secure = true;
        leftSideCookie.HttpOnly = true;

        Cookie rightSideCookie = transformer.CreateRightSideRequestCookie(leftSideCookie, "/somepath", true);

        Assert.IsNotNull(rightSideCookie, "Cookie is null");
        Assert.AreEqual("TestName", rightSideCookie.Name);
        Assert.AreEqual("TestValue|anotherValue", rightSideCookie.Value);
        Assert.AreEqual("/application", rightSideCookie.Path);
        Assert.AreEqual(inOneHour, rightSideCookie.Expires);
        Assert.AreEqual("www.portal.gv.at", rightSideCookie.Domain);
        Assert.IsTrue(rightSideCookie.Secure, "Secure");
        Assert.IsTrue(rightSideCookie.HttpOnly, "HttpOnly");
    }

    [Test]
    public void RightSideCookie_FromLeftSideCookieWithDomainAndPathCaseCorrection()
    {
        CookieTransformer transformer = new CookieTransformer(true, "https://somehost/somepath/");
        HttpCookie leftSideCookie = new HttpCookie("somehost/somepath/TestName",
                                                    CookieTransformer.c_CookieSignature +
                                                    "|www.portal.gv.at|/Application|TestValue|anotherValue");
        DateTime inOneHour = DateTime.Now + new TimeSpan(1, 0, 0);
        leftSideCookie.Domain = "www.domain.com";
        leftSideCookie.Path = "/somepath";
        leftSideCookie.Expires = inOneHour;
        leftSideCookie.Secure = true;
        leftSideCookie.HttpOnly = true;

        Cookie rightSideCookie = transformer.CreateRightSideRequestCookie(leftSideCookie, "/application", true);

        Assert.IsNotNull(rightSideCookie, "Cookie is null");
        Assert.AreEqual("TestName", rightSideCookie.Name);
        Assert.AreEqual("TestValue|anotherValue", rightSideCookie.Value);
        Assert.AreEqual("/application", rightSideCookie.Path);
        Assert.AreEqual(inOneHour, rightSideCookie.Expires);
        Assert.AreEqual("www.portal.gv.at", rightSideCookie.Domain);
        Assert.IsTrue(rightSideCookie.Secure, "Secure");
        Assert.IsTrue(rightSideCookie.HttpOnly, "HttpOnly");
    }

    [Test]
    public void RightSideCookie_FromLeftSideCookieWithDomainAndPathNotSecure()
    {
        CookieTransformer transformer = new CookieTransformer(true, "http://somehost/somepath/");
        HttpCookie leftSideCookie = new HttpCookie("somehost/somepath/TestName",
                                                    CookieTransformer.c_CookieSignature +
                                                    "|www.portal.gv.at|/application|TestValue|anotherValue");
        DateTime inOneHour = DateTime.Now + new TimeSpan(1, 0, 0);
        leftSideCookie.Domain = "www.domain.com";
        leftSideCookie.Path = "/somepath";
        leftSideCookie.Expires = inOneHour;
        leftSideCookie.Secure = true;
        leftSideCookie.HttpOnly = true;

        Cookie rightSideCookie = transformer.CreateRightSideRequestCookie(leftSideCookie, "/somepath", false);

        Assert.IsNotNull(rightSideCookie, "Cookie is null");
        Assert.AreEqual("TestName", rightSideCookie.Name);
        Assert.AreEqual("TestValue|anotherValue", rightSideCookie.Value);
        Assert.AreEqual("/application", rightSideCookie.Path);
        Assert.AreEqual(inOneHour, rightSideCookie.Expires);
        Assert.AreEqual("www.portal.gv.at", rightSideCookie.Domain);
        Assert.IsFalse(rightSideCookie.Secure, "Secure");
        Assert.IsTrue(rightSideCookie.HttpOnly, "HttpOnly");
    }

        [Test]
    public void RightSideCookie_FromInvalidSignatureLeftSideCookie()
    {
      CookieTransformer transformer = new CookieTransformer(true, "https://somehost/somepath/");
      HttpCookie leftSideCookie = new HttpCookie("TestName",
                                                 "invalidsignature" +
                                                 "|www.portal.gv.at|/application|TestValue|anotherValue");
      DateTime inOneHour = DateTime.Now + new TimeSpan(1, 0, 0);
      leftSideCookie.Domain = "www.domain.com";
      leftSideCookie.Path = "/somepath";
      leftSideCookie.Expires = inOneHour;
      leftSideCookie.Secure = true;
      leftSideCookie.HttpOnly = true;

      Cookie rightSideCookie = transformer.CreateRightSideRequestCookie(leftSideCookie, "/somepath", true);

      Assert.IsNull(rightSideCookie, "Cookie is not null");
    }

    [Test]
    public void RightSideCookie_FromInvalidFormatLeftSideCookie()
    {
      CookieTransformer transformer = new CookieTransformer(true, "https://somehost/somepath/");
      HttpCookie leftSideCookie = new HttpCookie("TestName",
                                                 CookieTransformer.c_CookieSignature +
                                                 "www.portal.gv.at|/application|TestValue|anotherValue");
      DateTime inOneHour = DateTime.Now + new TimeSpan(1, 0, 0);
      leftSideCookie.Domain = "www.domain.com";
      leftSideCookie.Path = "/somepath";
      leftSideCookie.Expires = inOneHour;
      leftSideCookie.Secure = true;
      leftSideCookie.HttpOnly = true;

      Cookie rightSideCookie = transformer.CreateRightSideRequestCookie(leftSideCookie, "/somepath", true);

      Assert.IsNull(rightSideCookie, "Cookie is not null");
    }

    [Test]
    public void RightSideCookie_FromLeftSideCookieWithNoDomainAndNoPath()
    {
      CookieTransformer transformer = new CookieTransformer(true, "https://somehost/somepath/");
      HttpCookie leftSideCookie = new HttpCookie("somehost/somepath/TestName",
                                                 CookieTransformer.c_CookieSignature +
                                                 "||/|TestValue|anotherValue");
      DateTime inOneHour = DateTime.Now + new TimeSpan(1, 0, 0);
      leftSideCookie.Domain = "www.domain.com";
      leftSideCookie.Path = "/somepath";
      leftSideCookie.Expires = inOneHour;
      leftSideCookie.Secure = true;
      leftSideCookie.HttpOnly = true;

      Cookie rightSideCookie = transformer.CreateRightSideRequestCookie(leftSideCookie, "/somepath", true);

      Assert.IsNotNull(rightSideCookie, "Cookie is null");
      Assert.AreEqual("TestName", rightSideCookie.Name);
      Assert.AreEqual("TestValue|anotherValue", rightSideCookie.Value);
      Assert.AreEqual("/", rightSideCookie.Path);
      Assert.AreEqual(inOneHour, rightSideCookie.Expires);
      Assert.IsEmpty(rightSideCookie.Domain, "Domain");
      Assert.IsTrue(rightSideCookie.Secure, "Secure");
      Assert.IsTrue(rightSideCookie.HttpOnly, "HttpOnly");
    }
  }
}