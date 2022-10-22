/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System.Collections.Specialized;
using System.Xml;
using Egora.Pvp;
using Egora.Stammportal.HttpReverseProxy.Mapping;
using Egora.Stammportal.HttpReverseProxy.StreamFilter;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests
{
  [TestFixture]
  public class CustomAuthorizationTest
  {
    [Test]
    public void SecClassTest()
    {
      CustomAuthorization auth = new CustomAuthorization(new HttpHeader[] { new HttpHeader("dummyName", "dummyValue") });
      Assert.That(auth.SecClass, Is.Null);

      auth = new CustomAuthorization();
      Assert.That(auth.SecClass, Is.Null);

      auth = new CustomAuthorization(new HttpHeader[] { new HttpHeader("dummyName", "dummyValue"), new HttpHeader("X-PVP-SECCLASS", "3") });
      Assert.That(auth.SecClass, Is.EqualTo("3"));

      auth = new CustomAuthorization(new HttpHeader[] { new HttpHeader("dummyName", "dummyValue"), new HttpHeader("X-AUTHENTICATE-gvSecClass", "1") });
      Assert.That(auth.SecClass, Is.EqualTo("1"));

      auth = new CustomAuthorization();
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(
        @"<pvpToken version=""1.9"" xmlns=""http://egov.gv.at/pvp1.xsd"">
<authenticate>
<participantId>AT:L6:994</participantId>
<userPrincipal>
<userId>fmeier@stmk.gv.at</userId>
<cn>F. Meier</cn>
<gvOuId>AT:L6:1299</gvOuId>
<ou>L6AL-F2/c</ou>
<mail>fmeier@stmk.gv.at</mail>
<tel>fmeier@stmk.gv.at</tel>
<gvSecClass>2</gvSecClass>
<gvGid>AT:B:0:UhO5RG++klaOTsVY+CU=</gvGid>
<gvFunction>SB</gvFunction>
</userPrincipal>
</authenticate>
<authorize>
<role value=""ZMR-Fremdenbehoerdenanfrage"">
<param>
<key>GKZ</key>
<value>60100</value>
</param>
</role>
</authorize>
</pvpToken>");
      auth.SoapHeaderXmlFragment = doc.DocumentElement;
      Assert.That(auth.SecClass, Is.EqualTo("2"));
    }
  }
}