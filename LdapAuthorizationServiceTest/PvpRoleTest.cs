/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using Egora.Stammportal.LdapAuthorizationService;
using NUnit.Framework;

namespace LdapAuthorizationServiceTest
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

      PvpRole role2 = PvpRole.Parse("Rolle2(Name2=Value2)");
      Assert.IsNotNull(role2);
      Assert.AreEqual("Rolle2", role2.Name);
      Assert.AreEqual("Value2", role2.Parameters["Name2"]);
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
    }
  }
}