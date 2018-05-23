/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Egora.Pvp;
using Egora.PvpHttpModule;
using NUnit.Framework;
using System.Collections.Specialized;

namespace Egora.PvpHttpModule.Test
{
  [TestFixture]
  public class PrincipalTest
  {
    [Test]
    public void PrincipalIdentityTest()
    {
      NameValueCollection headers = new NameValueCollection();
      headers.Add("X-Version", "1.9");
      headers.Add("X-AUTHENTICATE-cn", "Max Muster");
      headers.Add("X-AUTHENTICATE-userid", "max.muster@egora.at");
      headers.Add("X-AUTHENTICATE-gvouid", "123456");
      headers.Add("X-AUTHENTICATE-ou", "egora");
      headers.Add("X-AUTHORIZE-roles", "Test(GKZ=12)");
      headers.Add("X-AUTHORIZE-roles", "Test(GKZ=15)");
      string[] test = headers.GetValues("X-AUTHORIZE-roles");
      PvpPrincipal principal = new PvpPrincipal(headers);
      Assert.IsInstanceOf<IPvpPrincipal>(principal);
      Assert.AreEqual("123456", principal.GvOuId);
      Assert.AreEqual("egora", principal.Ou);
      var roles = principal.GetRoles();
      IIdentity identity = principal.Identity;
      Assert.IsNotNull(identity);
      Assert.AreEqual("PVP Version 1.9", identity.AuthenticationType);
      Assert.AreEqual("max.muster@egora.at", identity.Name);
      Assert.AreEqual(true, identity.IsAuthenticated);
    }

    [Test]
    public void PrincipalTestMissingHeader()
    {
      NameValueCollection headers = new NameValueCollection();
      headers.Add("somekey", "1.9");
      headers.Add("anotherkey", "Max Muster");
      PvpPrincipal principal = new PvpPrincipal(headers);
      Assert.IsInstanceOf<IPvpPrincipal>(principal);
      IIdentity identity = principal.Identity;
      Assert.IsNotNull(identity);
      Assert.IsNull(identity.AuthenticationType);
      Assert.IsNull(identity.Name);
      Assert.AreEqual(false, identity.IsAuthenticated);
    }
  }
}