/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Xml;
using Egora.Pvp;
using Egora.Stammportal.LdapAuthorizationService;
using NUnit.Framework;

namespace Egora.stammportal.LdapAuthorizationServiceTest
{
  [TestFixture]
  public class AuthorizationTest
  {
    [Test]
    public void NoApplication()
    {
      PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer("dummy", "dummy");
      Assert.IsNotNull(authorizer);
      Assert.IsFalse(authorizer.IsValid);
      Assert.IsFalse(authorizer.IsWeb);
      Assert.IsFalse(authorizer.IsSoap);
    }

    [Test]
    public void NoPvpAttribute()
    {
      PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer("http://noattribute.test", "egora2");
      Assert.IsNotNull(authorizer);
      Assert.AreEqual("egora.zwei@egora.at", authorizer.Mail);
      Assert.IsTrue(authorizer.IsValid);
      Assert.IsTrue(authorizer.IsWeb);
      Assert.IsFalse(authorizer.IsSoap);
    }
    [Test]
    public void AuthorizationSimple()
    {
      PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer("http://test.rubicon-it.com",
                                                                                 @"egora1");
      Assert.IsNotNull(authorizer);
      Assert.IsTrue(authorizer.IsValid);
      var fragment = authorizer.UserPrincipalSoapFragment;
      var ns = new XmlNamespaceManager(new NameTable());
      ns.AddNamespace("pvp", PvpToken.PvpTokenNamespace );
      var userId = fragment.SelectSingleNode("//pvp:userId", ns);
      Assert.AreEqual("egora.eins@egora.at", userId.InnerText);
      Assert.AreEqual("egora.eins@egora.at", authorizer.Mail, "MailAddress");
      Assert.AreEqual("&<>\"'ZMR-Behoerdenabfrage_(&GKZ=&1234)", authorizer.Roles, "Roles");
      Assert.AreEqual("Vienna", authorizer.CostCenterId);
      Assert.AreEqual("egora/Development", authorizer.ChargeCode);
      Assert.AreEqual(600, authorizer.AuthorizationTimeToLive, "TimeToLive");
      Assert.AreEqual(
        "<role value=\"&amp;&lt;&gt;&quot;&apos;ZMR-Behoerdenabfrage_\">\n<param>\n<key>&amp;GKZ</key><value>&amp;1234</value>\n</param>\n</role>",
        authorizer.GetPvpToken().RoleAttribute.GetXmlPart(), "SoapRoles");
      Assert.IsTrue(authorizer.GetAttributeValue(PvpAttributes.X_AUTHENTICATE_cn).EndsWith(" through formatter"));
      Assert.AreEqual("1.8", authorizer.Version);
    }

    [Test]
    public void AuthorizationSimpleNoRole()
    {
      PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer("http://testrole.rubicon-it.com",
                                                                                 @"rubicon\peter.grassnigg");
      Assert.IsNotNull(authorizer);
      Assert.IsFalse(authorizer.IsValid);
    }

    [Test]
    public void AuthorizationSimpleMustHaveRole()
    {
        PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer("http://testrole.rubicon-it.com",
                                                                                    @"rubicon\werner.kugler");
        Assert.IsNotNull(authorizer);
        Assert.IsNotNull(authorizer.User);
        Assert.IsNotNull(authorizer.Roles);
        Assert.Greater(authorizer.Roles.Length, 0);
        Assert.IsTrue(authorizer.IsValid);
    }

    [Test]
    public void AuthorizationRecursive()
    {
      PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer("http://testr.rubicon-it.com",
                                                                                 @"rubicon\egora2");
      Assert.AreEqual("1.9", authorizer.Version);
      Assert.IsNotNull(authorizer);
      Assert.IsTrue(authorizer.IsValid);
      Assert.AreEqual("egora.zwei@egora.at", authorizer.Mail, "MailAddress");
      Assert.AreEqual("EineRolle;TestRolle", authorizer.Roles, "Roles");
      Assert.AreEqual(
        "canonical int.rubicon-it.com/Development/egora/egora Zwei",
        authorizer.Ou, "with format.");

      Assert.AreEqual(500, authorizer.AuthorizationTimeToLive, "TimeToLive");

      string outerXml = authorizer.UserPrincipalSoapFragment.OuterXml;
      Assert.IsTrue(outerXml.StartsWith("<pvpToken version=\"1.9\" xmlns=\"http://egov.gv.at/pvp1.xsd\"><authenticate><participantId>Max.Mustermann</participantId>"));
      
      string userPrincipal = outerXml.Substring(outerXml.IndexOf("<userPrincipal>"));
      Assert.IsTrue(userPrincipal.Contains("<userId>egora.zwei@egora.at</userId>"));
      Assert.IsTrue(userPrincipal.Contains("<cn>egora Zwei</cn>"));
      Assert.IsTrue(userPrincipal.Contains("<ou>canonical int.rubicon-it.com/Development/egora/egora Zwei</ou>"));
      Assert.IsTrue(userPrincipal.Contains("<mail>egora.zwei@egora.at</mail>"));
      Assert.IsTrue(userPrincipal.Contains("<tel>Wien, DW 0815</tel>"));

      string authorize = outerXml.Substring(outerXml.IndexOf("<authorize>"));
      Assert.IsTrue(authorize.Contains("<role value=\"TestRolle\"></role>"));

    }

    [Test]
    public void NoAuthorizationRecursive()
    {
      PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer("http://testnr.rubicon-it.com",
                                                                                 @"egora.drei");
      Assert.IsNotNull(authorizer);
      Assert.IsTrue(authorizer.IsValid);
      Assert.AreEqual("egora.drei@egora.at", authorizer.Mail, "MailAddress");
      Assert.IsNull(authorizer.Roles, "Roles");
      Assert.AreEqual("Test", authorizer.Ou, "OU");
    }

    [Test]
    public void FixedRoleAttribute()
    {
      PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer("https://dummy.com/fixedrole/", "egora2");
      Assert.IsNotNull(authorizer);
      Assert.AreEqual("egora.zwei@egora.at", authorizer.Mail);
      Assert.IsTrue(authorizer.IsValid);
      Assert.IsFalse(authorizer.IsWeb);
      Assert.IsTrue(authorizer.IsSoap);
      Assert.That(authorizer.Roles, Is.EqualTo("FixedRole(param=val)"));

      var chainedToken = authorizer.GetPvpToken().GetChainedSoapFragment();

    }

    [Test]
    public void OuPathFormatter()
    {
      string path =
        "CN=Test Teresa,OU=OU 2,OU=Soziales,OU=Magistratsabteilung II,OU=Stadt Innsbruck,DC=intra,DC=ibk";
      Stammportal.LdapAuthorizationService.OuPathFormatter formatter = new OuPathFormatter();

      Assert.AreEqual("OU_2/Soziales/Magistratsabteilung_II/Stadt_Innsbruck", formatter.Format(path));
    }

    [Test]
    public void AdditionalAttribute()
    {
      PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer("http://test.rubicon-it.com",
                                                                                 @"egora.drei");
      Assert.AreEqual("egora Drei through formatter", authorizer.GetAttributeValue(PvpAttributes.X_AUTHENTICATE_cn));
    }

    [Test]
    public void OuPathSourceTest()
    {
      PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer("http://test.rubicon-it.com",
                                                                                 @"rubicon\bmi-pvp-user-1");
      Assert.AreEqual("Vienna", authorizer.GetAttributeValue(PvpAttributes.COST_CENTER_ID));
    }

    [Test]
    public void OuPathFormatterTest()
    {
      PvpApplicationLdapAuthorizer authorizer = new PvpApplicationLdapAuthorizer("http://test.rubicon-it.com",
                                                                                 @"rubicon\bmi-pvp-user-1");
      Assert.AreEqual("ServiceUser/egora/Development", authorizer.GetAttributeValue(PvpAttributes.CHARGE_CODE));
    }

    [Test]
    public void HashFormatterTest()
    {
      var hashFormatter = new Hash8Formatter();

      Assert.AreEqual("e/Smp8Ub", hashFormatter.Format(@"rubicon\test.user"));
      Assert.AreEqual("", hashFormatter.Format(null));
      Assert.AreEqual("2jmj7l5r", hashFormatter.Format(String.Empty));
    }
  }
}