using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Egora.Stammportal.PvpIdentityProvider;
using NUnit.Framework;

namespace Authentication.UnitTest
{
  [TestFixture]
  public class EntityInformationTest
  {
    [Test]
    public void ConfigFileTest()
    {
      var config = new EntityConfiguration();
      var entityInfo1 = new EntityInformation()
        { EntityID = "Entity1", FriendlyName = "Name1", SecClass = 2, SpInitiatedUrl = "url1" };
      var entityInfo2 = new EntityInformation()
        { EntityID = "Entity2", FriendlyName = "Name2", SecClass = 3, SpInitiatedUrl = "url2" };
      config.Entities = new EntityInformation[] { entityInfo1, entityInfo2 };
      var s = new XmlSerializer(typeof(EntityConfiguration));
      using (var fileStream = File.OpenWrite("Test.xml"))
        s.Serialize(fileStream, config);

      var config2 = (EntityConfiguration) s.Deserialize(File.OpenText("Test.xml"));
      Assert.That(config2.Entities.Length == 2);
      Assert.That(config2.Entities[0].EntityID, Is.EqualTo(entityInfo1.EntityID));
      Assert.That(config2.Entities[1].EntityID, Is.EqualTo(entityInfo2.EntityID));
    }
  }
}
