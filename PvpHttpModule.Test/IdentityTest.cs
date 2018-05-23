/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Text;
using Egora.Pvp;
using Egora.PvpHttpModule;
using NUnit.Framework;
using System.Collections.Specialized;

namespace Egora.PvpHttpModule.Test
{
  [TestFixture]
  public class IdentityTest
  {
    [Test]
    public void IdentityTestOK()
    {
      NameValueCollection headers = new NameValueCollection();
      headers.Add("X-Version", "1.9");
      headers.Add("X-AUTHENTICATE-cn", "Max Muster");
      headers.Add("X-AUTHENTICATE-userid", "max.muster@egora.at");
      headers.Add("X-AUTHENTICATE-gvouid", "123456");
      headers.Add("X-AUTHENTICATE-ou", "egora");
      IPvpIdentity identity = new PvpToken(headers);
      Assert.IsInstanceOf<IPvpIdentity>(identity);
      Assert.AreEqual("PVP Version 1.9", identity.AuthenticationType);
      Assert.AreEqual("max.muster@egora.at", identity.Name);
      Assert.AreEqual(true, identity.IsAuthenticated);
      Assert.AreEqual("123456", identity.GvOuId);
      Assert.AreEqual("egora", identity.Ou);

    }
    [Test]
    public void IdentityTestMissingHeader()
    {
      NameValueCollection headers = new NameValueCollection();
      headers.Add("somekey", "1.9");
      headers.Add("anotherkey", "Max Muster");
      IPvpIdentity identity = new PvpToken(headers);
      Assert.IsInstanceOf<IPvpIdentity>(identity);
      Assert.IsNull(identity.AuthenticationType);
      Assert.IsNull(identity.Name);
      Assert.AreEqual(false, identity.IsAuthenticated);
    }
  }
}