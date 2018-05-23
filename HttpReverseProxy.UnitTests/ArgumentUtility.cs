/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;

namespace Egora.Stammportal.HttpReverseProxy.UnitTests
{
  /// <summary>
  /// This utility class provides methods for checking arguments.
  /// </summary>
  internal static class ArgumentUtility
  {
    public static void CheckNotNull(string argumentName, object actualValue)
    {
      if (actualValue == null)
        throw new ArgumentNullException(argumentName);
    }

    public static void CheckNotNullOrEmpty(string argumentName, string actualValue)
    {
      CheckNotNull(argumentName, actualValue);
      if (actualValue.Length == 0)
        throw new ArgumentException("Argument is empty.", argumentName);
    }
  }
}