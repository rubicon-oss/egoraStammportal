/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System.Xml.Serialization;
using Egora.Stammportal.HttpReverseProxy.Mapping;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests.MappingTest
{
  [TestFixture]
  public class MappingTest
  {
    [Test]
    public void CreateMapping()
    {
      XmlSerializer serializer = new XmlSerializer(typeof (PathMap));
      PathMap mapping = (PathMap) serializer.Deserialize(
                                    new System.IO.StreamReader(
                                      @"MappingTest\Mapping.xml"));

      Assert.IsNotNull(mapping);
      Assert.AreEqual(11, mapping.Directories.Length);

      Directory dirroot = mapping.GetDirectory("/index.html");
      Assert.IsNotNull(dirroot, "dirroot not found.");
      Assert.AreEqual("", dirroot.Name);

      Directory dir1 = mapping.GetDirectory("/zmrgui/wasweissich.j");
      Assert.IsNotNull(dir1, "dir1 not found.");
      Assert.AreEqual("zmrgui", dir1.Name);

      Directory dir2 = mapping.GetDirectory("/zmrgui/sub1/waswei/ssich.j");
      Assert.IsNotNull(dir2, "dir2 not found.");
      Assert.AreEqual("sub1", dir2.Name);

      Directory dir3 = mapping.GetDirectory("/zmrgui/sub1/sub12/waswei/ssich.j");
      Assert.IsNotNull(dir3, "dir3 not found.");
      Assert.AreEqual("sub12", dir3.Name);
      Assert.AreEqual("https://portal.bmi.gv.at/portal/zmr-gw/sub1target/sub12/waswei/ssich.j",
                      dir3.GetFullTargetPath("/zmrgui/sub1/sub12/waswei/ssich.j"));

      Directory dir4 = mapping.GetDirectory("/caseCheck/Sub1/foo");
      Assert.IsNotNull(dir4, "dir4 not found.");
      Assert.AreEqual("sub1", dir4.Name);
      Assert.AreEqual("http://someserver/casecheck/sub1/waswei/ssich.j",
                      dir4.GetFullTargetPath("/Casecheck/sub1/waswei/ssich.j").ToLowerInvariant());

      ApplicationDirectory chain = mapping.GetApplication("/chaining/wasweissich.j");
      Assert.IsNotNull(chain, "App not found.");
      Assert.AreEqual("https://portal.bmi.gv.at/chaining/", chain.RootUrl);
      Assert.AreEqual(PvpTokenHandling.chain, chain.PvpInformationHandling);

      ApplicationDirectory app = mapping.GetApplication("/zmrgui/wasweissich.j");
      Assert.IsNotNull(app, "App not found.");
      Assert.AreEqual("https://portal.bmi.gv.at/portal/zmr-gw/", app.RootUrl);
      Assert.AreEqual(app.IsolateCookies, true);

      Directory dirStatistik = mapping.GetDirectory("/statistik.at");
      Assert.IsNotNull(dirStatistik, "dirStatistik not found.");
      Assert.AreEqual("statistik.at", dirStatistik.Name);
      Assert.AreEqual("/statistik.at/xyz", dirStatistik.GetFullTargetPath("/statistik.at/xyz"));

      ApplicationDirectory localsoap = mapping.GetApplication("/localsoap1/someAction");
      Assert.IsNotNull(localsoap, "App not found.");
      Assert.IsTrue(localsoap.LogTraffic);
    }

    [Test]
    public void RightSideUrl()
    {
      XmlSerializer serializer = new XmlSerializer(typeof (PathMap));
      PathMap mapping = (PathMap) serializer.Deserialize(
                                    new System.IO.StreamReader(
                                      @"MappingTest\Mapping.xml"));

      Directory dir1 = mapping.GetDirectory("/zmrsoap");
      Assert.IsNotNull(dir1, "dir1 not found.");
      Assert.AreEqual("zmrsoap", dir1.Name);
      Assert.AreEqual(
        "https://portals-test.bmi.gv.at/bmi.gv.at/soapv2/soaphttpengine/soapv2%23pvp1?dest=ZMR&opzone=test",
        dir1.GetFullTargetPath("/zmrsoap/"));

      ApplicationDirectory appStatistik = mapping.GetApplication("/statistik.at/vis.test.extern/");
      Assert.IsNotNull(appStatistik, "appStatistik not found.");
      Assert.AreEqual("vis.test.extern", appStatistik.Name);
      Assert.AreEqual("https://awp.statistik.at/statistik.at/vis.test.extern/", appStatistik.RootUrl);
      Assert.AreEqual("https://awp.statistik.at/statistik.at/vis.test.extern/einFile.txt",
                      appStatistik.GetFullTargetPath("/statistik.at/vis.test.extern/einFile.txt"));

      ApplicationDirectory withPort = mapping.GetApplication("/withport/");
      Assert.IsNotNull(withPort, "withport not found.");
      Assert.AreEqual("https://egora:8443/test", withPort.FullTargetPath);
      Assert.AreEqual("https://egora:8443/test/hello.htm", withPort.GetFullTargetPath("/withport/hello.htm"));

      ApplicationDirectory defaultApp = mapping.GetApplication("/someunknownurl/");
      Assert.IsNotNull(defaultApp, "defaultApp not found.");
      Assert.AreEqual(string.Empty, defaultApp.Name);
    }

    [Test]
    public void ByPassTest()
    {
      XmlSerializer serializer = new XmlSerializer(typeof (PathMap));
      PathMap mapping = (PathMap) serializer.Deserialize(
        new System.IO.StreamReader(
          @"MappingTest\Mapping.xml"));

      ApplicationDirectory zmrres = (ApplicationDirectory) mapping.GetDirectory("/zmrres");
      Assert.IsTrue(zmrres.ByPass("someStylesheet.css"));
      Assert.IsTrue(zmrres.ByPass("someStylesheet.css?somQueryPar"));
      Assert.IsFalse(zmrres.ByPass("anUrlwithcsswithin.txt"));
      Assert.IsFalse(zmrres.ByPass("anUrlwith.csswithin.txt"));

      Assert.IsTrue(zmrres.ByPass("(/images/somePicture.png"));
    }
  }
}