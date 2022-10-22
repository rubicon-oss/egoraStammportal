/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Diagnostics;
using System.Net;
using System.Security.Policy;
using System.Xml.Serialization;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests.IntegrationTest
{
  [TestFixture]
  public class SecClass3Test
  {
    [Test]
    public void RequestApplication3()
    {
      var appPage = "http://egoratest/stammportal/localtest3/TestPage.aspx";
      HttpWebRequest request1 = (HttpWebRequest) WebRequest.Create(appPage);
      CookieContainer cookieContainer = new CookieContainer();
      request1.CookieContainer = cookieContainer;
      request1.UseDefaultCredentials = true;
      request1.AllowAutoRedirect = false;
      HttpWebResponse response1 = (HttpWebResponse) request1.GetResponse();

      Assert.IsNotNull(response1, "Response1 is null");
      Assert.AreEqual(HttpStatusCode.Redirect, response1.StatusCode);
      var location1 = response1.Headers["Location"];
      StringAssert.Contains("/Authenticate/Authorize", location1);
      
      var request2 = (HttpWebRequest)WebRequest.Create(location1);
      cookieContainer.Add(response1.Cookies);
      request2.CookieContainer = cookieContainer;
      request2.AllowAutoRedirect = false;
      HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse();

      Assert.IsNotNull(response2, "Response2 is null");
      Assert.AreEqual(HttpStatusCode.Redirect, response2.StatusCode);
      var location2 = response2.Headers["Location"];

      var request3 = (HttpWebRequest)WebRequest.Create(location2);
      cookieContainer.Add(response2.Cookies);
      request3.CookieContainer = cookieContainer;
      request3.AllowAutoRedirect = false;
      HttpWebResponse response3 = (HttpWebResponse)request3.GetResponse();

      Assert.IsNotNull(response3, "Response3 is null");
      Assert.AreEqual(HttpStatusCode.Redirect, response3.StatusCode);
      var location3 = response3.Headers["Location"];
      var uriStart = new Uri(appPage);
      var uriEnd = new Uri(location3);
      Assert.That(uriEnd.Scheme, Is.EqualTo(uriStart.Scheme));
      Assert.That(uriEnd.Authority, Is.EqualTo(uriStart.Authority));
      Assert.That(uriEnd.PathAndQuery, Is.EqualTo(uriStart.PathAndQuery));
      Cookie authCookie = response3.Cookies["ExternalAuthentication"];
      Assert.IsNotNull(authCookie);
    }

  }
}