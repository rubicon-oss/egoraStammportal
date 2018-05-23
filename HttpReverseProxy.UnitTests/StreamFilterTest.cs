/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using Egora.Pvp;
using Egora.Stammportal.HttpReverseProxy.Mapping;
using Egora.Stammportal.HttpReverseProxy.StreamFilter;
using NUnit.Framework;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests
{
  [TestFixture]
  public class CopyFilterTest
  {
    [Test]
    public void CopyFilterWithLength()
    {
      CopyFilter filter = new CopyFilter(10);
      MemoryStream input = new MemoryStream(10);
      input.Write(Encoding.UTF8.GetBytes("1234567890"), 0, 10);
      input.Position = 0;
      MemoryStream output = new MemoryStream(10);
      filter.FilterStream(input, output);
      output.Position = 0;
      Assert.AreEqual(10, output.Length);
      string result = Encoding.UTF8.GetString(output.ToArray());
      Assert.AreEqual("1234567890", result);
    }

    [Test]
    public void CopyFilterWithLengthLimit()
    {
      CopyFilter filter = new CopyFilter(8);
      MemoryStream input = new MemoryStream(10);
      input.Write(Encoding.UTF8.GetBytes("1234567890"), 0, 10);
      input.Position = 0;
      MemoryStream output = new MemoryStream(10);
      filter.FilterStream(input, output);
      output.Position = 0;
      Assert.AreEqual(8, output.Length);
      string result = Encoding.UTF8.GetString(output.ToArray());
      Assert.AreEqual("12345678", result);
    }

    [Test]
    public void CopyFilterWithLengthTooHigh()
    {
      CopyFilter filter = new CopyFilter(12);
      MemoryStream input = new MemoryStream(10);
      input.Write(Encoding.UTF8.GetBytes("1234567890"), 0, 10);
      input.Position = 0;
      MemoryStream output = new MemoryStream(10);
      filter.FilterStream(input, output);
      output.Position = 0;
      Assert.AreEqual(10, output.Length);
      string result = Encoding.UTF8.GetString(output.ToArray());
      Assert.AreEqual("1234567890", result);
    }

    [Test]
    public void CopyFilterWithZeroLength()
    {
      CopyFilter filter = new CopyFilter(0);
      MemoryStream input = new MemoryStream(10);
      input.Write(Encoding.UTF8.GetBytes("1234567890"), 0, 10);
      input.Position = 0;
      MemoryStream output = new MemoryStream(10);
      filter.FilterStream(input, output);
      output.Position = 0;
      Assert.AreEqual(10, output.Length);
      string result = Encoding.UTF8.GetString(output.ToArray());
      Assert.AreEqual("1234567890", result);
    }

    [Test]
    public void CopyFilterLargeBuffer()
    {
      int length = 10000;
      CopyFilter filter = new CopyFilter(0);
      MemoryStream input = new MemoryStream(length);
      input.Write(new byte[length] , 0, length);
      input.Position = 0;
      MemoryStream output = new MemoryStream(length);
      filter.FilterStream(input, output);
      output.Position = 0;
      Assert.AreEqual(10000, output.Length);
    }
  }
}