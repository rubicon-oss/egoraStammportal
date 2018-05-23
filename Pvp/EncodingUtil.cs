/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Egora.Pvp
{
  public class EncodingUtil
  {
    public static string XmlEncode(string value)
    {
      StringBuilder sb = new StringBuilder(value);
      sb.Replace ("&", "&amp;");
      sb.Replace ("'", "&apos;");
      sb.Replace ("\"", "&quot;");
      sb.Replace (">", "&gt;");
      sb.Replace ("<", "&lt;");

      return sb.ToString ();
    }
  }
}
