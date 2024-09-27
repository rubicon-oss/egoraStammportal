using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Egora.Stammportal.Authentication;
using NUnit.Framework;

namespace Authentication.UnitTest
{
  [TestFixture]  
  public class JsonTest
    {
      [Test]
      public void SerializationTest()
      {
        var um = new UserMapping();
        var ms = new List<Mapping>();
        um.Mappings = ms;
        var m = new Mapping();
        ms.Add(m);
        m.IdAustria = new IdAustriaIdentity() {DateOfBirth = new DateTime(1968, 2, 28), FamilyName = "Kugler", GivenName = "Werner" };
        m.Ad = new AdIdentity() { GivenName = "testuser", Sn = "rubicon" };
        
        var result = JsonSerializer.Serialize(ms, new JsonSerializerOptions { IncludeFields = true });
      }

      [Test]
      public void DeserializeTest()
      {
        string input = @"

  { ""Mappings"" : 
  [
    {
      ""IdAustria"" : {
        ""GivenName"" : ""Werner"",
        ""FamilyName"" : ""Kugler"",
        ""DateOfBirth"" : ""1968-02-28T00:00:00""
      },
      ""Ad"" : {
        ""GivenName"" : ""testuser"",
        ""Sn"" : ""rubicon""
      }
    }
  ] 
  } 
";
        var mapping = JsonSerializer.Deserialize<UserMapping>(input, new JsonSerializerOptions() { IncludeFields = true });
        Assert.That(mapping.Mappings, Is.Not.Null);
      }

    }
  public class UserMapping
  {
    public List<Mapping> Mappings;
  }

  public class Mapping
  {
    public IdAustriaIdentity IdAustria;
    public AdIdentity Ad;
  }

  public class IdAustriaIdentity
  {
    public string GivenName;
    public string FamilyName;
    public DateTime DateOfBirth;
  }

  public class AdIdentity
  {
    public string GivenName;
    public string Sn;
  }
}
