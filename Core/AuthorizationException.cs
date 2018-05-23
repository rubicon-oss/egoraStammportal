/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;

namespace Egora.Stammportal.HttpReverseProxy
{
  public class AuthorizationException : Exception
  {
    public AuthorizationException()
    {
    }

    public AuthorizationException(string message)
      : base(message)
    {
    }

    public AuthorizationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public AuthorizationException(System.Runtime.Serialization.SerializationInfo info,
                                  System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    {
    }
  }
}