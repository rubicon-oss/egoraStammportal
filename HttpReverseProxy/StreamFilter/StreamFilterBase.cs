/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.IO;

namespace Egora.Stammportal.HttpReverseProxy.StreamFilter
{
  public class StreamFilterBase
  {
    public virtual long FilterStream(Stream fromStream, Stream toStream)
    {
      throw new NotImplementedException();
    }
  }
}