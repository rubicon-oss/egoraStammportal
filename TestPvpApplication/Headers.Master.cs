/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/
using System;

namespace TestPvpApplication
{
  public class Head
  {
    public Head(string name, string val)
    {
      _name = name;
      _value = val;
    }

    private string _name;

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    private string _value;

    public string Value
    {
      get { return _value; }
      set { _value = value; }
    }
  }

  public partial class Headers : System.Web.UI.MasterPage
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      Head[] headers = new Head[Request.Headers.Count];

      for (int i = 0; i < Request.Headers.Count; i++)
        headers[i] = new Head(Request.Headers.AllKeys[i], Request.Headers[i]);
      HeaderGridView.DataSource = headers;
      HeaderGridView.DataBind();
    }
  }
}