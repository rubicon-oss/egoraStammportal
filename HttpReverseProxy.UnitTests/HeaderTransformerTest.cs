/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using Egora.Stammportal.HttpReverseProxy.Mapping;
using Egora.Stammportal.HttpReverseProxy.StreamFilter;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests
{
  [TestFixture]
  public class HeaderTransformerTest
  {
    [Test]
    [Ignore] // test no longer works since Framwork 3.0 installed
    public void RightSideHeader_FromCustomLeftSideHeader()
    {
      PathMap map =
        PathMap.CreateFromFile(
          @"MappingTest\Mapping.xml");
      RemoteApplication.Initialize(map);

      HttpContext context =
        HttpContextHelper.CreateHttpContext("GET", "/localtest1/TestPage.aspx/someinfo", "name1=value1");


      RemoteApplication remoteApplication = RemoteApplication.GetRemoteApplication(context.Request);
      Assert.IsNotNull(remoteApplication, "remoteApplication");

      TrafficLogger logger = new TrafficLogger(remoteApplication.RemoteApplicationProxyPath, "UnitTest", new TraceScope(null), context.Request.Url);
      NameValueCollection headers = new NameValueCollection();
      headers.Add("X-Custom", "CustomValue");
      headers.Add("Range", "messages=1-20,25-30");
      HttpRequestHelper.AddHeaders(context.Request, headers);
      using (Stream inputBuffer = CopyFilter.GetInputStream(context.Request))
      {
        HttpWebRequest webRequest = remoteApplication.CreateRightSideRequest(context.Request, inputBuffer, logger);
        Assert.IsNotNull(webRequest, "Request is null.");
        Assert.AreEqual("GET", webRequest.Method);
        Assert.IsNotNull(webRequest.Headers["X-Custom"], "CustomHeader");
        Assert.AreEqual("CustomValue", webRequest.Headers["X-Custom"]);
        Assert.IsNotNull(webRequest.Headers["Range"], "Range");
        Assert.AreEqual("messages=1-20,25-30", webRequest.Headers["Range"]);
      }
    }

    [Test]
    public void LocationHeaderTest()
    {
      string location =
        "http://localhost:9090/fkb2local/views/client.api.DatenImportierenLandesIT.jsf?FT=X";

      PathTransformer transformer = new PathTransformer("http://localhost:9090/fkb2local/", "/bmi.gv.at/fk2web-t/");

      Assert.AreEqual("/bmi.gv.at/fk2web-t/views/client.api.DatenImportierenLandesIT.jsf?FT=X", transformer.AdjustPath(location));
    }

    [Test]
    public void LocationHeaderTest2()
    {
        string location =
            "http://localhost:9090/someurl.do?someParam=http://localhost:9090/someotherurl.do";
        string expectedLoction = location;

        PathTransformer transformer = new PathTransformer("https://pvawp.bmi.gv.at/bmi.gv.at/fk2web-t/", "/bmi.gv.at/fk2web-t/");

        Assert.AreEqual(expectedLoction, transformer.AdjustPath(location));
    }

    [Test]
    public void LocationHeaderTest3()
    {
        string location =
            "http://some.where/someapp/someurl.do?someParam=http://localhost:9090/someotherurl.do";
     
        PathTransformer transformer = new PathTransformer("https://some.where/someapp/", "/someapp/");

        Assert.AreEqual("/someapp/someurl.do?someParam=http://localhost:9090/someotherurl.do", transformer.AdjustPath(location));
    }
    
    [Test]
    public void LocationHeaderTest4()
    {
      string location = "/at.gv.bmdw.erecht-q/app/";

      PathTransformer transformer = new PathTransformer("https://awp.lfrz.at/at.gv.bmdw.erecht-q", "/at.gv.bmdw.erecht-q/");

      Assert.AreEqual("/at.gv.bmdw.erecht-q/app/", transformer.AdjustPath(location));
    }

    [Test]
    public void RemoveHeaderTest()
    {
      var transformer = new HeaderTransformerTestObject(true, "2.1");
      WebHeaderCollection coll1 = new WebHeaderCollection()
                                  {
                                    { "VsDebuggerCausalityData", "SomeValue" }
                                    , {"SomeNormalHeader", "aValue" }
                                  };
      WebHeaderCollection rightsideHeaders = transformer.TransformRequestHeaders(coll1);
      Assert.AreEqual(1, rightsideHeaders.Count);
      Assert.AreEqual("aValue", rightsideHeaders["SomeNormalHeader"]);
    }

    [Test]
    public void TxId19Test()
    {
      var transformer = new HeaderTransformerTestObject(PvpTokenHandling.chain, "1.9");
      WebHeaderCollection coll1 = new WebHeaderCollection()
                                  {
                                    {"X-TXID", "asdf" }
                                  };
      WebHeaderCollection rightsideHeaders = transformer.TransformRequestHeaders(coll1);
      Assert.AreEqual(1, rightsideHeaders.Count);
      Assert.AreEqual("asdf", rightsideHeaders["X-TXID"]);
    }
    [Test]
    public void TxId20Test()
    {
      var transformer = new HeaderTransformerTestObject(PvpTokenHandling.chain, "2.0");
      WebHeaderCollection coll1 = new WebHeaderCollection()
                                  {
                                    {"X-PVP-TXID", "asdf" }
                                  };
      WebHeaderCollection rightsideHeaders = transformer.TransformRequestHeaders(coll1);
      Assert.AreEqual(1, rightsideHeaders.Count);
      Assert.AreEqual("asdf", rightsideHeaders["X-PVP-TXID"]);
    }

    [Test]
    [Ignore]
    public void RangeTest()
    {
      HttpRequest leftRequest = new HttpRequest("dummy", "http://somewhere.com", "dummy");
      leftRequest.Headers["range"] = "bytes=300-";
      HttpWebRequest rightRequest = (HttpWebRequest) WebRequest.Create("http://nowhere.com/");
      HeaderTransformer transformer = new HeaderTransformer(leftRequest, rightRequest, PvpTokenHandling.chain, "dummy", "dummy", true, null, "1.9");
      transformer.Transform();
      Assert.AreEqual("bytes=300-", rightRequest.Headers["range"]);
    }

    [Test]
    public void Test19Headers()
    {
      var transformer = new HeaderTransformerTestObject(true, "1.9");
      WebHeaderCollection coll1 = new WebHeaderCollection() {{"a", "b"}};
      var rightsideHeaders = transformer.TestPvpHeaderTransformation(coll1);
      Assert.AreEqual(0, rightsideHeaders.Count);

      coll1 = new WebHeaderCollection()
                {
                  {"dummy", "nonsense"},
                  {"X-Version", "1.9"},
                  {"X-AUTHENTICATE-participantId", "AT:L6:1234789"},
                  {"X-AUTHENTICATE-UserId", "mmustermann@kommunalnet.at"},
                  {"X-AUTHENTICATE-cn", "Max Mustermann"},
                  {"X-AUTHENTICATE-gvGid", "AT:B:0:LxXnvpcYZesiqVXsZG0bB=="},
                  {"X-AUTHENTICATE-gvOuId", "AT:GGA-60420:0815"},
                  {"X-AUTHENTICATE-Ou", "Meldeamt"},
                  {"X-AUTHENTICATE-gvOuOKZ", "AT:GGA-60420-Abt13"},
                  {"X-AUTHENTICATE-mail", "max.mustermann@hatzendorf.steiermark.at"},
                  {"X-AUTHENTICATE-tel", "+43 3155 5153"},
                  {"X-AUTHENTICATE-gvSecClass", "2"},
                  {"X-AUTHORIZE-roles", "Beispielrolle(GKZ=60420)"},
                };
      rightsideHeaders = transformer.TestPvpHeaderTransformation(coll1);
      Assert.AreEqual(12, rightsideHeaders.Count);
      Assert.IsTrue(rightsideHeaders["X-01-AUTHENTICATE-participantId"] == "AT:L6:1234789");
      Assert.IsTrue(rightsideHeaders["X-01-AUTHENTICATE-UserId"] == "mmustermann@kommunalnet.at");
      Assert.IsTrue(rightsideHeaders["X-01-AUTHENTICATE-cn"] == "Max Mustermann");
      Assert.IsTrue(rightsideHeaders["X-01-AUTHENTICATE-gvGid"] == "AT:B:0:LxXnvpcYZesiqVXsZG0bB==");
      Assert.IsTrue(rightsideHeaders["X-01-AUTHENTICATE-gvOuId"] == "AT:GGA-60420:0815");
      Assert.IsTrue(rightsideHeaders["X-01-AUTHENTICATE-Ou"] == "Meldeamt");
      Assert.IsTrue(rightsideHeaders["X-01-AUTHENTICATE-gvOuOKZ"] == "AT:GGA-60420-Abt13");
      Assert.IsTrue(rightsideHeaders["X-01-AUTHENTICATE-mail"] == "max.mustermann@hatzendorf.steiermark.at");
      Assert.IsTrue(rightsideHeaders["X-01-AUTHENTICATE-tel"] == "+43 3155 5153");
      Assert.IsTrue(rightsideHeaders["X-01-AUTHENTICATE-gvSecClass"] == "2");
      Assert.IsTrue(rightsideHeaders["X-01-AUTHORIZE-roles"] == "Beispielrolle(GKZ=60420)");
    }

    [Test]
    public void Test21Headers()
    {
      var transformer = new HeaderTransformerTestObject(true, "2.1");
      WebHeaderCollection coll1 = new WebHeaderCollection() { { "a", "b" } };
      var rightsideHeaders = transformer.TestPvpHeaderTransformation(coll1);
      Assert.AreEqual(0, rightsideHeaders.Count);

      coll1 = new WebHeaderCollection()
                {
                  {"dummy", "nonsense"},
                  {"X-PVP-Version", "2.1"},
                  {"X-PVP-participantId", "AT:L6:1234789"},
                  {"X-PVP-UserId", "mmustermann@kommunalnet.at"},
                  {"X-PVP-PRINCIPAL-NAME", "Mustermann"},
                  {"X-PVP-gvGid", "AT:B:0:LxXnvpcYZesiqVXsZG0bB=="},
                  {"X-PVP-gvOuId", "AT:GGA-60420:0815"},
                  {"X-PVP-Ou", "Meldeamt"},
                  {"X-PVP-gvOuOKZ", "AT:GGA-60420-Abt13"},
                  {"X-PVP-mail", "max.mustermann@hatzendorf.steiermark.at"},
                  {"X-PVP-tel", "+43 3155 5153"},
                  {"X-PVP-gvSecClass", "2"},
                  {"X-PVP-roles", "Beispielrolle(GKZ=60420)"},
                };
      rightsideHeaders = transformer.TestPvpHeaderTransformation(coll1);
      Assert.AreEqual(11, rightsideHeaders.Count);
      Assert.IsTrue(rightsideHeaders["X-PVP-participantId_01"] == "AT:L6:1234789");
      Assert.IsTrue(rightsideHeaders["X-PVP-UserId_01"] == "mmustermann@kommunalnet.at");
      Assert.IsTrue(rightsideHeaders["X-PVP-PRINCIPAL-NAME_01"] == "Mustermann");
      Assert.IsTrue(rightsideHeaders["X-PVP-gvGid_01"] == "AT:B:0:LxXnvpcYZesiqVXsZG0bB==");
      Assert.IsTrue(rightsideHeaders["X-PVP-gvOuId_01"] == "AT:GGA-60420:0815");
      Assert.IsTrue(rightsideHeaders["X-PVP-Ou_01"] == "Meldeamt");
      Assert.IsTrue(rightsideHeaders["X-PVP-gvOuOKZ_01"] == "AT:GGA-60420-Abt13");
      Assert.IsTrue(rightsideHeaders["X-PVP-mail_01"] == "max.mustermann@hatzendorf.steiermark.at");
      Assert.IsTrue(rightsideHeaders["X-PVP-tel_01"] == "+43 3155 5153");
      Assert.IsTrue(rightsideHeaders["X-PVP-gvSecClass_01"] == "2");
      Assert.IsTrue(rightsideHeaders["X-PVP-roles_01"] == "Beispielrolle(GKZ=60420)");
    }
  }

  public class HeaderTransformerTestObject : HeaderTransformer
  {
    public HeaderTransformerTestObject(bool isolateCookies, string version) : base(new HttpRequest("","http://no.where/", null), (HttpWebRequest) WebRequest.Create("http://no.where/"), PvpTokenHandling.chain, "dummy", "dummy", isolateCookies, null, version)
    {
      
    }

    public HeaderTransformerTestObject(PvpTokenHandling pvpTokenHandling, string version) : base(new HttpRequest("", "http://no.where/", null), (HttpWebRequest)WebRequest.Create("http://no.where/"), pvpTokenHandling, "dummy", "dummy", false, null, version)
    {

    }
    public WebHeaderCollection TransformRequestHeaders(NameValueCollection leftSideHeaders)
    {
      WebHeaderCollection rightSideHeaders=new WebHeaderCollection();
      base.TransformRequestHeaders(leftSideHeaders, rightSideHeaders);
      return rightSideHeaders;
    }

    public WebHeaderCollection TestPvpHeaderTransformation(NameValueCollection leftsideHeaders)
    {
      var rightsideHeaders = new WebHeaderCollection();
      foreach (string header in leftsideHeaders)
      {
        base.HandlePvpHeader(header, leftsideHeaders[header], rightsideHeaders);
      }
      return rightsideHeaders;
    }
  }
}