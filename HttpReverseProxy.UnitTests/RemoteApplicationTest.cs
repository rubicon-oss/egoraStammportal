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
using System.Web;
using Egora.Stammportal.HttpReverseProxy.Mapping;
using Egora.Stammportal.HttpReverseProxy.StreamFilter;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests
{
  [TestFixture]
  public class RemoteApplicationTest
  {
    [Test]
    public void RemoteApplication_GetRequest()
    {
      PathMap map =
        PathMap.CreateFromFile(
          @"MappingTest\Mapping.xml");
      RemoteApplication.Initialize(map);

      HttpContext context = HttpContextHelper.CreateHttpContext("GET", "/localtest1/TestPage.aspx/someinfo", "name1=value1");
      using (new TraceScope(context))
      {
        RemoteApplication remoteApplication = RemoteApplication.GetRemoteApplication(context.Request);
        TrafficLogger logger = new TrafficLogger(remoteApplication.RemoteApplicationProxyPath, "UnitTest", new TraceScope(null), context.Request.Url);
        string rightSideUrl = remoteApplication.GetRightSideUrl(context.Request);
        Assert.AreEqual("http://egoratest/PvpTestApplication/1/TestPage.aspx/someinfo?name1=value1", rightSideUrl,
                        "RightSideUrl");

        using (Stream inputBuffer = CopyFilter.GetInputStream(context.Request))
        {
          HttpWebRequest request = remoteApplication.CreateRightSideRequest(context.Request, inputBuffer, logger);
          Assert.IsNotNull(request, "Request is null.");
          Assert.AreEqual("GET", request.Method);
        }
      }
    }

    [Test]
    public void RemoteApplication_PostRequest()
    {
      PathMap map =
        PathMap.CreateFromFile(
          @"MappingTest\Mapping.xml");
      RemoteApplication.Initialize(map);

      HttpContext context = HttpContextHelper.CreateHttpContext("POST", "/localtest1/", null);
      HttpRequest leftSideRequest = context.Request;

      RemoteApplication remoteApplication = RemoteApplication.GetRemoteApplication(context.Request);
      TrafficLogger logger = new TrafficLogger(remoteApplication.RemoteApplicationProxyPath, "UnitTest", new TraceScope(null), context.Request.Url);
      string rightSideUrl = remoteApplication.GetRightSideUrl(context.Request);
      Assert.AreEqual("http://egoratest/PvpTestApplication/1/", rightSideUrl, "RightSideUrl");

      using (Stream inputBuffer = CopyFilter.GetInputStream(context.Request))
      {
        HttpWebRequest rightSideRequest = remoteApplication.CreateRightSideRequest(context.Request, inputBuffer, logger);
        Assert.IsNotNull(rightSideRequest, "Request is null.");
        Assert.AreEqual("POST", rightSideRequest.Method);
        // currently empty collection
        foreach (HttpHeader header in leftSideRequest.Headers)
        {
          Assert.IsNotNull(rightSideRequest.Headers[header.Name], "header");
          Assert.AreEqual(header.Value, rightSideRequest.Headers[header.Name]);
        }
      }
    }

    [Test]
    public void RemoteApplication_ByPass()
    {
      PathMap map =
        PathMap.CreateFromFile(
          @"MappingTest\Mapping.xml");
      RemoteApplication.Initialize(map);

      HttpContext context = HttpContextHelper.CreateHttpContext("GET", "/zmrres/images/somepicture.png",
                                                                "name1=value1");

      RemoteApplication remoteApplication = RemoteApplication.GetRemoteApplication(context.Request);
      TrafficLogger logger = new TrafficLogger(remoteApplication.RemoteApplicationProxyPath, "UnitTest", new TraceScope(null), context.Request.Url);
      string rightSideUrl = remoteApplication.GetRightSideUrl(context.Request);
      Assert.AreEqual("https://portal.bmi.gv.at/images/somepicture.png?name1=value1", rightSideUrl,
                      "RightSideUrl");
      Assert.IsTrue(remoteApplication.ByPass(context.Request.Url.AbsolutePath));
      
      using (Stream inputBuffer = CopyFilter.GetInputStream(context.Request))
      {
        HttpWebRequest request = remoteApplication.CreateRightSideRequest(context.Request, inputBuffer, logger);
        Assert.IsNotNull(request, "Request is null.");
        Assert.AreEqual("GET", request.Method);
      }
    }

    [Test]
    public void RemoteApplication_GetRemoteApplication()
    {
      PathMap map =
        PathMap.CreateFromFile(
          @"MappingTest\Mapping.xml");
      RemoteApplication.Initialize(map);

      HttpContext context1 = HttpContextHelper.CreateHttpContext("GET", "/localtest/TestPage.aspx/someinfo",
                                                                 "name1=value1");

      RemoteApplication app1 = RemoteApplication.GetRemoteApplication(context1.Request);

      Assert.IsNotNull(app1, "RemoteApplication");

      HttpContext context2 = HttpContextHelper.CreateHttpContext("GET", "/somepath/TestPage.aspx/someinfo",
                                                                 "name1=value1");
      RemoteApplication app2 = RemoteApplication.GetRemoteApplication(context2.Request);

      Assert.IsNotNull(app2);
      Assert.AreEqual("https://someserver/", app2.RootUrl);
    }
  }
}