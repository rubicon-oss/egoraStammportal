/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Egora.Pvp.Attributes;
using NUnit.Framework;

namespace Egora.Pvp.Test
{
  [TestFixture]
  public class PvpAuthorizationTest
  {
    [Test]
    public void OneLineTest()
    {
      NameValueCollection headers = new NameValueCollection();
      headers.Add("X-Authorize-roles", "R1(p1=v1);R2(p1=v1,p1=v2,p2=v2)");
      headers.Add("X-Version", "1.9");
      PvpToken token = new PvpToken(headers);
      PvpAttributeRoles authorization = token.RoleAttribute;
      Assert.AreEqual(2, authorization.Roles.Count);
      PvpRole role1 = authorization.GetRole("r1");
      Assert.IsNotNull(role1);
      Assert.AreEqual(1, role1.Parameters.Count);
      Assert.AreEqual("p1", role1.Parameters.GetKey(0));
      Assert.AreEqual("v1", role1.Parameters["p1"]);

      PvpRole role2 = authorization.GetRole("r2");
      Assert.IsNotNull(role2);
      Assert.AreEqual(2, role2.Parameters.Count);
      Assert.AreEqual("p1", role2.Parameters.GetKey(0));
      Assert.AreEqual("v1,v2", role2.Parameters["p1"]);
      Assert.AreEqual("p2", role2.Parameters.GetKey(1));
      Assert.AreEqual("v2", role2.Parameters["p2"]);
    }

    [Test]
    public void TwoLinesTest()
    {
      NameValueCollection headers = new NameValueCollection();
      headers.Add("X-Version", "1.9");
      headers.Add("X-Authorize-roles", "R1(p1=v1);R2(p1=v1,p1=v2,p2=v2)");
      headers.Add("X-Authorize-roles", "R1(p1=v2);R3(p1=v1,p1=v2,p2=v2)");
      PvpToken token = new PvpToken(headers);
      PvpAttributeRoles authorization = token.RoleAttribute;
      Assert.AreEqual(3, authorization.Roles.Count);

      PvpRole role1 = authorization.GetRole("r1");
      Assert.IsNotNull(role1);
      Assert.AreEqual(1, role1.Parameters.Count);
      Assert.AreEqual("p1", role1.Parameters.GetKey(0));
      Assert.AreEqual("v1,v2", role1.Parameters["p1"]);

      PvpRole role2 = authorization.GetRole("r2");
      Assert.IsNotNull(role2);
      Assert.AreEqual(2, role2.Parameters.Count);
      Assert.AreEqual("p2", role2.Parameters.GetKey(1));
      Assert.AreEqual("v1,v2", role2.Parameters["p1"]);
      Assert.AreEqual("v2", role2.Parameters["p2"]);

      PvpRole role3 = authorization.GetRole("r3");
      Assert.IsNotNull(role3);
      Assert.AreEqual(2, role3.Parameters.Count);
      Assert.AreEqual("p1", role3.Parameters.GetKey(0));
      Assert.AreEqual("v1,v2", role3.Parameters["p1"]);
      Assert.AreEqual("v2", role2.Parameters["p2"]);
    }

    [Test]
    public void HasRoleTest()
    {
      PvpToken token = new PvpToken(PvpVersion.Version20);
      token.Attributes.Add(new PvpAttributeRoles("R1(p1=v1);R2(p1=v1,p1=v2,p2=v2);R3(GKZ)"));
      Assert.IsTrue(token.RoleAttribute.HasRole(new PvpRole("R1")));
      Assert.IsTrue(token.RoleAttribute.HasRole(PvpRole.Parse("R1(p1=v1)")));
      Assert.IsTrue(token.RoleAttribute.HasRole(PvpRole.Parse("R2(p1=v1,p1=v2,p2=v2)")));
      Assert.IsTrue(token.RoleAttribute.HasRole(new PvpRole("R2")));
      Assert.IsTrue(token.RoleAttribute.HasRole(PvpRole.Parse("R2()")));
      Assert.IsFalse(token.RoleAttribute.HasRole(PvpRole.Parse("R4")));
      Assert.IsFalse(token.RoleAttribute.HasRole(PvpRole.Parse("R1(p1=v2)")));
      Assert.IsFalse(token.RoleAttribute.HasRole(PvpRole.Parse("R1(p1=v2)")));
      Assert.IsTrue(token.RoleAttribute.HasRole(new PvpRole("R3")));
      Assert.IsTrue(token.RoleAttribute.HasRole(PvpRole.Parse("R3(Gkz)")));
    }
  }
}
