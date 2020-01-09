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
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests.IntegrationTest
{
  [TestFixture]
  public class HeaderTest
  {
    [Test]
    public void CustomHeader()
    {
      HttpWebRequest request = CreateRequest();
      request.Headers.Add("X-Custom", "SomeValue");
      HttpWebResponse response = (HttpWebResponse) request.GetResponse();

      Assert.IsNotNull(response, "Response");

      XmlSerializer serializer = new XmlSerializer(typeof (RequestInformation));
      RequestInformation info = (RequestInformation) serializer.Deserialize(response.GetResponseStream());

      Assert.IsNotNull(info, "RequestInformation");
      Assert.AreEqual("SomeValue", info.GetHeader("X-Custom"));

      Assert.IsNotNull(info.GetHeader("PVP-Header1"), "PVP-Header1");
      Assert.AreEqual("PVP-Value1", info.GetHeader("PVP-Header1"));
    }

    [Test]
    public void SessionIDAsUrlPartTest()
    {
      HttpWebRequest request = CreateRequestWithSessionIDAsUrlPart("fgq1l4y35bojgi45lthqkk3l");
      request.Headers.Add("X-Custom", "SomeValue");
      HttpWebResponse response = (HttpWebResponse)request.GetResponse();

      Assert.IsNotNull(response, "Response");

      XmlSerializer serializer = new XmlSerializer(typeof(RequestInformation));
      RequestInformation info = (RequestInformation)serializer.Deserialize(response.GetResponseStream());

      Assert.IsNotNull(info, "RequestInformation");
      Assert.AreEqual("SomeValue", info.GetHeader("X-Custom"));

      Assert.IsNotNull(info.GetHeader("PVP-Header1"), "PVP-Header1");
      Assert.AreEqual("PVP-Value1", info.GetHeader("PVP-Header1"));

      Assert.AreEqual("/Stammportal/localtest1/IntegrationTestPage.aspx", info.GetHeader("X-ORIG-URI"));
      Assert.AreEqual("S(fgq1l4y35bojgi45lthqkk3l)", info.GetHeader("AspFilterSessionId"));
    }


    [Test]
    [Explicit]
    public void SecureConnect()
    {
      HttpWebRequest request =
          (HttpWebRequest)WebRequest.Create("http://egoratest/Stammportal/localtestsecure/IntegrationTestPage.aspx");
      request.UseDefaultCredentials = true;

      request.Headers.Add("X-Custom", "SomeValue");
      HttpWebResponse response = null;
      try
      {
        response = (HttpWebResponse) request.GetResponse();
      }
      catch (WebException e)
      {
        var errorResponse = e.Response;
        using (var reader = new StreamReader(errorResponse.GetResponseStream(), Encoding.UTF8))
          Debug.WriteLine(reader.ReadToEnd());
      }
      Assert.IsNotNull(response, "Response");

      XmlSerializer serializer = new XmlSerializer(typeof(RequestInformation));
      RequestInformation info = (RequestInformation)serializer.Deserialize(response.GetResponseStream());

      Assert.IsNotNull(info, "RequestInformation");
      Assert.AreEqual("SomeValue", info.GetHeader("X-Custom"));
    }

    [Test]
    public void IgnoredHeader()
    {
      HttpWebRequest request = CreateRequest();
      request.Headers.Add("X-Custom", "SomeValue");
      request.Headers.Add("VsDebuggerCausalityData", "xyz");
      HttpWebResponse response = (HttpWebResponse)request.GetResponse();

      Assert.IsNotNull(response, "Response");

      XmlSerializer serializer = new XmlSerializer(typeof(RequestInformation));
      RequestInformation info = (RequestInformation)serializer.Deserialize(response.GetResponseStream());

      Assert.IsNotNull(info, "RequestInformation");
      Assert.AreEqual("SomeValue", info.GetHeader("X-Custom"));

      Assert.IsNull(info.GetHeader("VsDebuggerCausalityData"), "ignored Header returned.");
    }

    [Test]
    public void OriginHeader()
    {
      HttpWebRequest request = CreateRequest("somequerystring");
      HttpWebResponse response = (HttpWebResponse)request.GetResponse();

      Assert.IsNotNull(response, "Response");

      XmlSerializer serializer = new XmlSerializer(typeof(RequestInformation));
      RequestInformation info = (RequestInformation)serializer.Deserialize(response.GetResponseStream());

      Assert.IsNotNull(info, "RequestInformation");
      Assert.AreEqual("http", info.GetHeader("X-ORIG-SCHEME"));
      Assert.AreEqual("egoratest", info.GetHeader("X-ORIG-HOSTINFO"));
      Assert.AreEqual("/Stammportal/localtest1/IntegrationTestPage.aspx", info.GetHeader("X-ORIG-URI"));
    }

    [Test(Description = "could fail on hour change during test")]
    public void TxId()
    {
      HttpWebRequest request = CreateRequest();
      HttpWebResponse response = (HttpWebResponse)request.GetResponse();

      Assert.IsNotNull(response, "Response");

      XmlSerializer serializer = new XmlSerializer(typeof(RequestInformation));
      RequestInformation info = (RequestInformation)serializer.Deserialize(response.GetResponseStream());

      Assert.IsNotNull(info, "RequestInformation");
      string txid = info.GetHeader("X-TXID");
      string[] txParts = txid.Split("$@".ToCharArray());
      Assert.AreEqual(3, txParts.Length);
      Assert.AreEqual(DateTime.Now.ToString("yyyyMMdd_HH"), txParts[0].Substring(0,11));
      Assert.AreEqual("00", txParts[1].Substring(0,2)); //letzte stelle könnte mehr als 0 sein, wenn mehr als 1 request zur gleichen zeit kommt
      Assert.AreEqual("egoratest", txParts[2]);
    }

    [Test]
    public void FromHeader()
    {
      HttpWebRequest request = CreateRequest();
      request.Headers.Add("From", "Hugo");
      HttpWebResponse response = (HttpWebResponse)request.GetResponse();

      Assert.IsNotNull(response, "Response");

      XmlSerializer serializer = new XmlSerializer(typeof(RequestInformation));
      RequestInformation info = (RequestInformation)serializer.Deserialize(response.GetResponseStream());

      Assert.IsNotNull(info, "RequestInformation");
      Assert.IsNull(info.GetHeader("From")); // because header is removed
      Assert.AreEqual("Hugo", info.GetHeader("X-AUTHENTICATE-UserId"));

      Assert.IsNotNull(info.GetHeader("PVP-Header1"), "PVP-Header1");
      Assert.AreEqual("PVP-Value1", info.GetHeader("PVP-Header1"));
    }

    [Test]
    public void Chunked()
    {
        HttpWebRequest request = CreateRequest();
        request.SendChunked = true;
        request.Method = "POST";
        using (var s = request.GetRequestStream())
        {
            byte[] someText = Encoding.UTF8.GetBytes("SomeText");
            s.Write(someText, 0, someText.Length);
        } 
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        Assert.IsNotNull(response, "Response");

        XmlSerializer serializer = new XmlSerializer(typeof(RequestInformation));
        RequestInformation info = (RequestInformation)serializer.Deserialize(response.GetResponseStream());

        Assert.IsNotNull(info, "RequestInformation");
        Assert.AreEqual("chunked", info.GetHeader("Transfer-Encoding")); 

        Assert.IsNotNull(info.GetHeader("PVP-Header1"), "PVP-Header1");
        Assert.AreEqual("PVP-Value1", info.GetHeader("PVP-Header1"));
    }

    [Test]
    public void ByPass()
    {
      HttpWebRequest request =
        (HttpWebRequest)WebRequest.Create("http://egoratest/Stammportal/localtest1/IntegrationTestPage.aspx/ByPassToken");
      request.UseDefaultCredentials = true;
      request.Headers.Add("From", "Hugo");
      request.Headers.Add("MyHeader", "MyTestValue");
      HttpWebResponse response = (HttpWebResponse)request.GetResponse();

      Assert.IsNotNull(response, "Response");

      XmlSerializer serializer = new XmlSerializer(typeof(RequestInformation));
      RequestInformation info = (RequestInformation)serializer.Deserialize(response.GetResponseStream());

      Assert.IsNotNull(info, "RequestInformation");
      Assert.IsNull(info.GetHeader("From")); // because header is removed
      Assert.IsNull(info.GetHeader("X-AUTHENTICATE-UserId"));
      Assert.AreEqual("MyTestValue", info.GetHeader("MyHeader"));
    }

    private HttpWebRequest CreateRequest(string query)
    {
      HttpWebRequest request =
        (HttpWebRequest)WebRequest.Create("http://egoratest/Stammportal/localtest1/IntegrationTestPage.aspx" + (String.IsNullOrEmpty(query) ? "" : "?"+query));
      request.UseDefaultCredentials = true;
      return request;
    }

    private HttpWebRequest CreateRequestWithSessionIDAsUrlPart(string sessionID)
    {
      HttpWebRequest request =
        (HttpWebRequest)WebRequest.Create("http://egoratest/Stammportal/(S("+sessionID+"))/localtest1/IntegrationTestPage.aspx");
      request.UseDefaultCredentials = true;
      return request;
    }
    private HttpWebRequest CreateRequest()
    {
      return CreateRequest(null);
    }
  }
}