/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Net;
using System.Web;

namespace Egora.Stammportal.HttpReverseProxy
{
  public class RemoteApplicationHistory
  {
    public RemoteApplicationHistory(HttpRequest leftSideRequest, WebRequest rightSideRequest)
    {
      Path = leftSideRequest.Path;
      Method = leftSideRequest.HttpMethod;
      Target = rightSideRequest.RequestUri.OriginalString;
      _date = DateTime.Now;
    }

    private string _path;

    public string Path
    {
      get { return _path; }
      set { _path = value; }
    }

    private string _method;

    public string Method
    {
      get { return _method; }
      set { _method = value; }
    }

    private string _target;

    public string Target
    {
      get { return _target; }
      set { _target = value; }
    }

    private DateTime _date;

    public DateTime Date
    {
      get { return _date; }
    }
  }
}