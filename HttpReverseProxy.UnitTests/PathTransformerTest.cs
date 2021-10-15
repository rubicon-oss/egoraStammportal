/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests
{
  [TestFixture]
  public class PathTransformerTest
  {
    [Test]
    public void LeftSidePath_FromRightSideFullPath()
    {
      PathTransformer pathTransformer = new PathTransformer("http://somedomain/", "/somedomain/");
      string newPath = pathTransformer.AdjustPath("http://somedomain/page1.html");

      Assert.IsNotNull(newPath);
      Assert.AreEqual("/somedomain/page1.html", newPath);
    }

    [Test]
    public void LeftSidePath_FromRightSideSecureFullPath()
    {
      PathTransformer pathTransformer = new PathTransformer("https://somedomain/", "/somedomain/");
      string newPath = pathTransformer.AdjustPath("https://somedomain/page1.html");

      Assert.IsNotNull(newPath);
      Assert.AreEqual("/somedomain/page1.html", newPath);
    }

    [Test]
    public void LeftSidePath_FromRightRelativPath()
    {
      PathTransformer pathTransformer = new PathTransformer("http://somedomain/", "/somedomain/");
      string newPath = pathTransformer.AdjustPath("page1.html");

      Assert.IsNotNull(newPath);
      Assert.AreEqual("page1.html", newPath);
    }

    [Test]
    public void LeftSidePath_FromRightAbsolutPath()
    {
      PathTransformer pathTransformer = new PathTransformer("http://somedomain/", "/somedomain/");
      string newPath = pathTransformer.AdjustPath("/page1.html");

      Assert.IsNotNull(newPath);
      Assert.AreEqual("/somedomain/page1.html", newPath);
    }

    [Test]
    public void LeftSidePath_FromRightAbsolutPathAndNotRootProxyPath()
    {
      PathTransformer pathTransformer = new PathTransformer("https://www.fundamt.gv.at/gondor/", "/stammportal/fundamt/");
      string newPath = pathTransformer.AdjustPath("/gondor/WebGov/UI/Security/FormsLogin.aspx");

      Assert.IsNotNull(newPath);
      Assert.AreEqual("/stammportal/fundamt/WebGov/UI/Security/FormsLogin.aspx", newPath);
    }

    [Test]
    public void LeftSidePath_FromRightAbsolutPathAndApplicationAsSubdir()
    {
      PathTransformer pathTransformer =
        new PathTransformer("https://awp.statistik.at/statistik.at/vis.test.extern/",
                            "/statistik.at/vis.test.extern/");
      string newPath = pathTransformer.AdjustPath("https://awp.statistik.at/statistik.at/vis.test.extern/start.do");

      Assert.IsNotNull(newPath);
      Assert.AreEqual("/statistik.at/vis.test.extern/start.do", newPath);
    }
  }
}