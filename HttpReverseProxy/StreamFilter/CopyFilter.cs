/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using Egora.Stammportal.HttpReverseProxy.Properties;

namespace Egora.Stammportal.HttpReverseProxy.StreamFilter
{
  public class CopyFilter : StreamFilterBase
  {
    public CopyFilter(long streamLength)
    {
      _length = streamLength;
    }

    private const int _bufferSize = 8 * 1024;
    private long _length;

    public override long FilterStream(Stream fromStream, Stream toStream)
    {
      long counter = 0;
      int bytesRead = 0;
      byte[] buffer = new byte[(_bufferSize > _length) && (_length > 0) ? _length : _bufferSize];
      try
      {
        do
        {
          bytesRead = fromStream.Read(buffer, 0, buffer.Length);
          toStream.Write(buffer, 0, bytesRead);
          counter += bytesRead;
        } while (bytesRead > 0 && (_length<=0 || counter<_length));
      }
      catch (ObjectDisposedException ex)
      {
        TraceScope.Current.TraceEvent(TraceEventType.Error, (int)HttpReverseProxyHandler.Event.StreamFilter,
                                     "Stream disposed: {0}", ex.ObjectName);
      }
      toStream.Flush();

      return counter;
    }

    public static Stream GetInputStream(HttpRequest request)
    {
      if (Settings.Default.BufferLeftSide || Settings.Default.NetworkRetryCount > 0)
      {
        MemoryStream inputBuffer = new MemoryStream(request.ContentLength);
        CopyFilter copyFilter = new CopyFilter(request.ContentLength);
        copyFilter.FilterStream(request.InputStream, inputBuffer);
        inputBuffer.Position = 0;
        return inputBuffer;
      }
      return request.InputStream;
    }
  }
}