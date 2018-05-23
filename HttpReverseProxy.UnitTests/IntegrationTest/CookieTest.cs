/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System.Net;
using System.Xml.Serialization;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests.IntegrationTest
{
  [TestFixture]
  public class CookieTest
  {
    [Test]
    public void SetCookie()
    {
      HttpWebRequest request1 = (HttpWebRequest) WebRequest.Create(
                                                   "http://egoratest/stammportal/localtest1/IntegrationTestPage.aspx?CookieName=cname&CookieValue=cval&CookiePath=IntegrationTestPage.aspx&HttpOnly=true");
      CookieContainer cookieContainer = new CookieContainer();
      request1.CookieContainer = cookieContainer;
      request1.UseDefaultCredentials = true;
      HttpWebResponse response1 = (HttpWebResponse) request1.GetResponse();

      Assert.IsNotNull(response1, "Response");
      Assert.IsNotNull(response1.Cookies);
      Assert.AreEqual(1, response1.Cookies.Count);

      Cookie responseCookie = response1.Cookies[0];
      Assert.IsNotNull(responseCookie);
      Assert.AreEqual("egoratest/PvpTestApplication/1/cname", responseCookie.Name);
      Assert.AreEqual(
        CookieTransformer.c_CookieSignature + "|egoratest|/PvpTestApplication/1/IntegrationTestPage.aspx|cval",
        responseCookie.Value);
      Assert.AreEqual("/stammportal", responseCookie.Path);
      Assert.IsTrue(responseCookie.HttpOnly, "Cookie is not HttpOnly.");


      HttpWebRequest request2 = (HttpWebRequest) WebRequest.Create(
                                                   "http://egoratest/stammportal/localtest1/IntegrationTestPage.aspx");
      request2.CookieContainer = cookieContainer;
      request2.UseDefaultCredentials = true;
      HttpWebResponse response2 = (HttpWebResponse) request2.GetResponse();

      Assert.IsNotNull(response2, "Response");

      XmlSerializer serializer = new XmlSerializer(typeof (RequestInformation));
      RequestInformation info = (RequestInformation) serializer.Deserialize(response2.GetResponseStream());

      Assert.IsNotNull(info, "RequestInformation");
      Assert.IsNotNull(info.Cookies, "Cookies");

      CookieInformation cookie = info.GetCookie("cname");

      Assert.IsNotNull(cookie, "Cookie");
      Assert.AreEqual("cval", cookie.Value);
    }

    [Test]
    public void SetCookie_NoIsolation()
    {
      HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(
                                                   "http://egoratest/stammportal/localtest2/IntegrationTestPage.aspx?CookieName=cname&CookieValue=cval&CookiePath=IntegrationTestPage.aspx&HttpOnly=true");
      CookieContainer cookieContainer = new CookieContainer();
      request1.CookieContainer = cookieContainer;
      request1.UseDefaultCredentials = true;
      HttpWebResponse response1 = (HttpWebResponse)request1.GetResponse();

      Assert.IsNotNull(response1, "Response");
      Assert.IsNotNull(response1.Cookies);
      Assert.AreEqual(1, response1.Cookies.Count);

      Cookie responseCookie = response1.Cookies[0];
      Assert.IsNotNull(responseCookie);
      Assert.AreEqual("egoratest/cname", responseCookie.Name);
      Assert.AreEqual(
        CookieTransformer.c_CookieSignature + "|egoratest|/PvpTestApplication/2/IntegrationTestPage.aspx|cval",
        responseCookie.Value);
      Assert.AreEqual("/stammportal", responseCookie.Path);
      Assert.IsTrue(responseCookie.HttpOnly, "Cookie is not HttpOnly.");


      HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(
                                                   "http://egoratest/stammportal/localtest2/IntegrationTestPage.aspx");
      request2.CookieContainer = cookieContainer;
      request2.UseDefaultCredentials = true;
      HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse();

      Assert.IsNotNull(response2, "Response");

      XmlSerializer serializer = new XmlSerializer(typeof(RequestInformation));
      RequestInformation info = (RequestInformation)serializer.Deserialize(response2.GetResponseStream());

      Assert.IsNotNull(info, "RequestInformation");
      Assert.IsNotNull(info.Cookies, "Cookies");

      CookieInformation cookie = info.GetCookie("cname");

      Assert.IsNotNull(cookie, "Cookie");
      Assert.AreEqual("cval", cookie.Value);
    }
  }
}