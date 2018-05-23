using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Egora.Pvp.Attributes;
using NUnit.Framework;

namespace Egora.Pvp.Test
{
  [TestFixture]
  public class PvpAttributeTest
  {
    
    public class PvpAttributeDummy1Attribute :PvpAttribute
    {
      public PvpAttributeDummy1Attribute()
      : base(

        friendlyName: "DUMMY1"
        , index: PvpAttributes.USERID
        , samlAttributeName: "urn:oid:1.2.3.4.5.6"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                        {PvpVersion.Version20, "X-PVP-DUMMY1"}
                      }
      , soapElementName: null
        )
    {}
    }
    
    public class PvpAttributeDummy2Attribute : PvpAttribute
    {
      public PvpAttributeDummy2Attribute()
        : base(

          friendlyName: "DUMMY2"
          , index: PvpAttributes.USERID
          , samlAttributeName: "urn:oid:1.2.3.4.5.6"
          , availableInVersions: new PvpVersion[] { PvpVersion.Version21 }
          , headerNames: new Dictionary<PvpVersion, string>()
                      {
                        {PvpVersion.Version20, "X-PVP-DUMMY2"}
                       ,{PvpVersion.Version21, "X-PVP-DUMMY2"}
                      }
      , soapElementName: null
          )
      { }
    }

    [Test]
    [ExpectedException(ExpectedException = typeof(PvpException), ExpectedMessage = "PvpAttribute DUMMY1 für Version Version21 definiert, aber kein HeaderName für diese Version definiert.")]
    public void Initialize1Test()
    {
      new PvpAttributeDummy1Attribute();
    }

    [Test]
    [ExpectedException(ExpectedException = typeof(PvpException), ExpectedMessage = "HeaderName für Version Version20 definiert, aber PvpAttribute DUMMY2 nicht für diese Version definiert.")]
    public void Initialize2Test()
    {
      new PvpAttributeDummy2Attribute();
    }

    [Test]
    [ExpectedException(ExpectedException = typeof(PvpException), ExpectedMessage = "Version 1.1 wird nicht unterstützt.")]
    public void VersionTest()
    {
      var v = new PvpAttributeVersion(PvpVersionNumber.Version18);
      Assert.AreEqual("1.8", v.Value);
      v = new PvpAttributeVersion(PvpVersionNumber.Version19);
      Assert.AreEqual("1.9", v.Value);
      v = new PvpAttributeVersion(PvpVersionNumber.Version20);
      Assert.AreEqual("2.0", v.Value);
      v = new PvpAttributeVersion(PvpVersionNumber.Version21);
      Assert.AreEqual("2.1", v.Value);

      new PvpAttributeVersion("1.1");
    }

    [Test]
    [ExpectedException(ExpectedException = typeof (PvpException),
      ExpectedMessage = "1234-13-01 ist kein gültiges Datum.")]
    public void BirthdateTest()
    {
      var d = new PvpAttributeBirthDate("1990-01-01");
      Assert.AreEqual("1990-01-01", d.Value);
      d= new PvpAttributeBirthDate("2000-00-00");
      Assert.AreEqual("2000-00-00", d.Value);

      new PvpAttributeBirthDate("1234-13-01");
    }
  }
}
