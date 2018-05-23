/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

namespace Egora.Stammportal
{
  /// <summary>
  /// Holds Information for HttpHeaders.
  /// </summary>
  public class HttpHeader
  {
    public HttpHeader()
    {
    }

    /// <summary>
    /// Creates a new Instance with given name and value.
    /// </summary>
    /// <param name="name">Name of the header</param>
    /// <param name="value">Value of the header</param>
    public HttpHeader(string name, string value)
    {
      _name = name;
      _value = value;
    }

    private string _name;
    private string _value;

    /// <summary>
    /// Gets or sets the name of the header.
    /// </summary>
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    /// <summary>
    /// Gets or sets the value of the header
    /// </summary>
    public string Value
    {
      get { return _value; }
      set { _value = value; }
    }
  }
}