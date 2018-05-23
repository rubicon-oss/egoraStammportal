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
using System.Text;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests.IntegrationTest
{
  [TestFixture]
  public class RetryTest
  {
    [Test]
    [Ignore]
    // When connection is closed, framework tries again once automatically
    // Always sends response with IIS 7.5 and .NET 3.5
    public void CloseConnectionPageDirect()
    {
      HttpWebRequest request1 = (HttpWebRequest) WebRequest.Create(
                                                   "http://egoratest/PvpTestApplication/CloseConnection.aspx?reset=true");
      request1.UseDefaultCredentials = true;
      HttpWebResponse response1 = (HttpWebResponse) request1.GetResponse();

      Assert.IsNotNull(response1, "Response");
      Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode);

      HttpWebRequest request2 = (HttpWebRequest) WebRequest.Create(
                                                   "http://egoratest/PvpTestApplication/CloseConnection.aspx");
      request2.UseDefaultCredentials = true;
      bool exceptionThrown = false;
      try
      {
        HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse();
      }
      catch (WebException)
      {
        exceptionThrown = true;
      }

      Assert.IsTrue(exceptionThrown, "There is a response.");

    }

    [Test]
    public void Error500PageDirect()
    {
      HttpWebRequest request1 = (HttpWebRequest) WebRequest.Create(
                                                   "http://egoratest/PvpTestApplication/1/Error500.aspx?reset=true");
      request1.UseDefaultCredentials = true;
      HttpWebResponse response1 = (HttpWebResponse) request1.GetResponse();

      Assert.IsNotNull(response1, "Response");
      Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode);

      HttpWebRequest request2 = (HttpWebRequest) WebRequest.Create(
                                                   "http://egoratest/PvpTestApplication/1/Error500.aspx");
      request2.UseDefaultCredentials = true;
      bool exceptionThrown = false;
      try
      {
        HttpWebResponse response2 = (HttpWebResponse) request2.GetResponse();
      }
      catch (WebException e)
      {
        exceptionThrown =true;
      }
      Assert.IsTrue(exceptionThrown,"No Exception received.");
    }

    [Test]
    public void CloseConnectionPage()
    {
      HttpWebRequest request1 = (HttpWebRequest) WebRequest.Create(
                                                   "http://egoratest/stammportal/localtest1/CloseConnection.aspx?reset=true");
      request1.UseDefaultCredentials = true;
      HttpWebResponse response1 = (HttpWebResponse) request1.GetResponse();

      Assert.IsNotNull(response1, "Response");
      Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode);

      HttpWebRequest request2 = (HttpWebRequest) WebRequest.Create(
                                                   "http://egoratest/stammportal/localtest1/CloseConnection.aspx");
      request2.UseDefaultCredentials = true;
      HttpWebResponse response2 = (HttpWebResponse) request2.GetResponse();

      Assert.IsNotNull(response2, "Response");
      Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode);
    }

    [Test]
    public void Error500Page()
    {
      HttpWebRequest request1 = (HttpWebRequest) WebRequest.Create(
                                                   "http://egoratest/stammportal/localtest1/Error500.aspx?reset=true");
      request1.UseDefaultCredentials = true;
      HttpWebResponse response1 = (HttpWebResponse) request1.GetResponse();

      Assert.IsNotNull(response1, "Response");
      Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode);

      HttpWebRequest request2 = (HttpWebRequest) WebRequest.Create(
                                                   "http://egoratest/stammportal/localtest1/Error500.aspx");
      request2.UseDefaultCredentials = true;
      HttpWebResponse response2 = (HttpWebResponse) request2.GetResponse();

      Assert.IsNotNull(response2, "Response");
      Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode);
    }
  }
}