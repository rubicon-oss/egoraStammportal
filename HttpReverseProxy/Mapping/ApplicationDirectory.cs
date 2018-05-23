/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Egora.Stammportal.HttpReverseProxy.Mapping
{
  public class ApplicationDirectory : Directory
  {
    private string _rootUrl;
    private bool _rootUrlHasQuery = false;

    [XmlAttribute]
    public string RootUrl
    {
      get { return _rootUrl; }
      set
      {
        _rootUrl = value;
        _rootUrlHasQuery = _rootUrl.Contains("?");
      }
    }

    private string _certificateFile;

    [XmlAttribute]
    public string CertificateFile
    {
      get { return _certificateFile; }
      set { _certificateFile = value; }
    }

    private string _authorizationWebService;

    [XmlAttribute]
    public string AuthorizationWebService
    {
      get { return _authorizationWebService; }
      set { _authorizationWebService = value; }
    }

    private PvpTokenHandling _pvpInformationHandling;

    [XmlAttribute]
    public PvpTokenHandling PvpInformationHandling
    {
      get { return _pvpInformationHandling; }
      set { _pvpInformationHandling = value; }
    }

    private bool _logTraffic;
    private string _byPassExpression;
    private Regex _byPassRegex = null;
    private bool _isolateCookies = true;

    [XmlAttribute]
    public bool LogTraffic 
    {
      get { return _logTraffic; }
      set { _logTraffic = value; }
    }

    [XmlAttribute]
    public bool IsolateCookies
    {
      get { return _isolateCookies; }
      set { _isolateCookies = value; }
    }

    [XmlIgnore]
    public override ApplicationDirectory Application
    {
      get { return this; }
    }

    [XmlIgnore]
    public override string FullTargetPath
    {
      get
      {
        if (RootUrl.EndsWith("/"))
        {
          return RootUrl.Substring(0, RootUrl.Length - 1);
        }
        return RootUrl;
      }
    }

    [XmlAttribute]
    public string ByPassExpression
    {
      get { return _byPassExpression; }
      set
      {
        _byPassExpression = value;
        _byPassRegex = new Regex(_byPassExpression, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
      }
    }

    public override string GetFullTargetPath(string fullPath)
    {
      if (!fullPath.StartsWith(FullPath, Comparison))
        throw new ApplicationException("Wrong path.");

      if (_rootUrlHasQuery)
      {
        return FullTargetPath;
      }

      return base.GetFullTargetPath(fullPath);
    }

    public virtual bool ByPass(string url)
    {
      return (_byPassRegex != null && _byPassRegex.IsMatch(url));
    }
  }
}