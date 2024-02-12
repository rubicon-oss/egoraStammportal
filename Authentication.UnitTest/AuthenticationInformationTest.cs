using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Egora.Stammportal.Authentication;
using NUnit.Framework;

namespace Authentication.UnitTest
{
  [TestFixture]  
  public class AuthenticationInformationTest
    {
      [Test]
      public void SerializationTest()
      {
        var auth1 = new AuthenticationInformation() { SecClass = 2, UserName = "dummy" };
        var base64 = auth1.ToBase64String();
        Assert.IsNotNullOrEmpty(base64);
        var auth2 = AuthenticationInformation.FromBase64String(base64);
        Assert.IsNotNull(auth2);
        Assert.AreEqual(auth1.SecClass, auth2.SecClass);
        Assert.AreEqual(auth1.UserName, auth2.UserName);
      }
  }
}
