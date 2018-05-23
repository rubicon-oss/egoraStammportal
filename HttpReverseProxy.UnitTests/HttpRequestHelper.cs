/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System.Collections.Specialized;
using System.Web;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests
{
  /// <summary> 
  ///   Provides helper methods for initalizing an <see cref="HttpContext"/> object when simulating ASP.NET request
  ///   cycles. 
  /// </summary>
  public class HttpRequestHelper
  {
    public static void SetHeaders(HttpRequest httpRequest, NameValueCollection headers)
    {
      NameValueCollection currentHeaders = httpRequest.Headers;
      PrivateInvoke.InvokeNonPublicMethod(currentHeaders, "MakeReadWrite");
      currentHeaders.Clear();
      currentHeaders.Add(headers);
      PrivateInvoke.InvokeNonPublicMethod(currentHeaders, "MakeReadOnly");
    }

    public static void AddHeaders(HttpRequest httpRequest, NameValueCollection headers)
    {
      NameValueCollection currentHeaders = httpRequest.Headers;
      // funktioniert nicht mehr
      PrivateInvoke.InvokeNonPublicMethod(currentHeaders, "MakeReadWrite");
      currentHeaders.Add(headers);
      PrivateInvoke.InvokeNonPublicMethod(currentHeaders, "MakeReadOnly");
    }
  }
}