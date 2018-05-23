/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using Egora.Pvp;
using NUnit.Framework;

namespace Egora.Pvp.Test
{
  [TestFixture]
  public class PvpRoleTest
  {
    [Test]
    public void RoleWithoutParamater()
    {
      PvpRole role1 = PvpRole.Parse("Rolle1");
      Assert.IsNotNull(role1);
      Assert.AreEqual("Rolle1", role1.Name);

      PvpRole role2 = PvpRole.Parse("Rolle2()");
      Assert.IsNotNull(role2);
      Assert.AreEqual("Rolle2", role2.Name);
    }

    [Test]
    public void RoleWithOneParamater()
    {
      PvpRole role1 = PvpRole.Parse("Rolle1(Name1)");
      Assert.IsNotNull(role1);
      Assert.AreEqual("Rolle1", role1.Name);
      Assert.AreEqual("Name1", role1.Parameters.Keys[0]);
      Assert.AreEqual("<role value=\"Rolle1\">\n<param>\n<key>Name1</key>\n</param>\n</role>",
        role1.GetSoapFragment());

      PvpRole role2 = PvpRole.Parse("Rolle2(Name2=Value2)");
      Assert.IsNotNull(role2);
      Assert.AreEqual("Rolle2", role2.Name);
      Assert.AreEqual("Value2", role2.Parameters["Name2"]);
      Assert.AreEqual("<role value=\"Rolle2\">\n<param>\n<key>Name2</key><value>Value2</value>\n</param>\n</role>",
        role2.GetSoapFragment());
    }

    [Test]
    public void RoleWithTwoParamaters()
    {
      PvpRole role1 = PvpRole.Parse("Rolle1(Name1=Value1,Name2=Value2)");
      Assert.IsNotNull(role1);
      Assert.AreEqual("Rolle1", role1.Name);
      Assert.AreEqual("Value1", role1.Parameters["Name1"]);
      Assert.AreEqual("Value2", role1.Parameters["Name2"]);
      Assert.AreEqual("Rolle1(Name1=Value1,Name2=Value2)", role1.ToString());
      Assert.AreEqual("<role value=\"Rolle1\">\n<param>\n<key>Name1</key><value>Value1</value>\n</param>\n<param>\n<key>Name2</key><value>Value2</value>\n</param>\n</role>", 
        role1.GetSoapFragment());
    }

    [Test]
    public void RoleWithThreeParamaters()
    {
      PvpRole role1 = PvpRole.Parse("Rolle1(Name1=Value1,Name2,Name3=Value3)");
      Assert.IsNotNull(role1);
      Assert.AreEqual("Rolle1", role1.Name);
      Assert.AreEqual("Value1", role1.Parameters["Name1"]);
      Assert.AreEqual("Name2", role1.Parameters.Keys[1]);
      Assert.AreEqual("Value3", role1.Parameters["Name3"]);
      Assert.AreEqual("Rolle1(Name1=Value1,Name2,Name3=Value3)", role1.ToString());
      Assert.AreEqual("<role value=\"Rolle1\">\n<param>\n<key>Name1</key><value>Value1</value>\n</param>\n<param>\n<key>Name2</key>\n</param>\n<param>\n<key>Name3</key><value>Value3</value>\n</param>\n</role>",
        role1.GetSoapFragment());
    }

    [Test]
    public void DecodeTest()
    {
      PvpRole role1 = PvpRole.Parse(@"\\R\(o\)lle\s1\,\;(Nam\s\,e1=Value1,Name2,Name3=Value3)", true);
      Assert.AreEqual(@"\R(o)lle 1,;", role1.Name);
      Assert.AreEqual("Nam ,e1", role1.Parameters.GetKey(0));
      Assert.IsNull(role1.GetValuesForParameter("Name2"));
      Assert.AreEqual("Value3", role1.Parameters["Name3"]);
    }

    [Test]
    public void DecodeTestWithoutEscapedCharacter()
    {
      PvpRole role1 = PvpRole.Parse(@"Rolle1(Name1=Value1,Name2,Name3=Value3)", true);
      Assert.AreEqual(@"Rolle1", role1.Name);
      Assert.AreEqual("Name1", role1.Parameters.GetKey(0));
      Assert.IsNull(role1.GetValuesForParameter("Name2"));
      Assert.AreEqual("Value3", role1.Parameters["Name3"]);
    }
  }
}