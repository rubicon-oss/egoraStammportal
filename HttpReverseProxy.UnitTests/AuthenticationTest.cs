/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using Egora.Stammportal.HttpReverseProxy.Mapping;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests
{
  //[TestFixture]
  public class AuthenticationTest
  {
    [Test]
    public void UriTest()
    {
      var uri = new Uri("https://server/dir1/dir2/file.ext?name1=valuewith%2F");
      var redirect = uri.PathAndQuery;
      Assert.That(System.Web.HttpUtility.UrlEncode(redirect), Is.EqualTo("%2fdir1%2fdir2%2ffile.ext%3fname1%3dvaluewith%252F"));
    }
  }
}