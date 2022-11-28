/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Egora.Stammportal.HttpReverseProxy.Mapping;
using Egora.Stammportal.HttpReverseProxy.Properties;

namespace Egora.Stammportal.HttpReverseProxy
{
  public class RemoteApplication
  {
    public enum Event
    {
      AssociateApplication = 200,
      CreateRemoteApplication,
      CalculateRightSideUrl,
      CreateRightSideRequest,
    }

    private static PathMap s_map;

    private static Dictionary<ApplicationDirectory, RemoteApplication> s_remoteApplications =
      new Dictionary<ApplicationDirectory, RemoteApplication>();

    public static int HistoryMaxLength = Properties.Settings.Default.HistoryLength;

    internal static RemoteApplication[] GetActiveApplications()
    {
      RemoteApplication[] apps = new RemoteApplication[s_remoteApplications.Count];
      s_remoteApplications.Values.CopyTo(apps, 0);
      return apps;
    }

    public static void Initialize(PathMap map)
    {
      lock (s_remoteApplications)
      {
        s_map = map;
        s_remoteApplications.Clear();
        AuthorizationWebServiceProxy.Initialize();
      }
    }

    public static RemoteApplication GetRemoteApplication(HttpRequest leftSideRequest)
    {
      string path = leftSideRequest.Path.Substring(leftSideRequest.ApplicationPath.Length);
      if (!path.StartsWith("/"))
        path = "/" + path;

      ApplicationDirectory appDir = s_map.GetApplication(path);
      if (appDir != null)
      {
        RemoteApplication app;
        lock (s_remoteApplications)
        {
          if (s_remoteApplications.ContainsKey(appDir))
          {
            app = s_remoteApplications[appDir];
          }
          else
          {
            app = new RemoteApplication(appDir);
            s_remoteApplications.Add(appDir, app);
          }
        }
        TraceScope.Current.TraceEvent(TraceEventType.Verbose, (int) Event.AssociateApplication,
                                     "Found RemoteApplication {0} for path {1}.", app.RootUrl, path);
        return app;
      }
      return null;
    }

    public static RemoteApplication GetRemoteApplicationByTarget(string targetRootUrl)
    {
      var apps = GetActiveApplications();
      foreach (var app in apps)
      {
        if (app.RootUrl.Equals(targetRootUrl, StringComparison.InvariantCultureIgnoreCase))
          return app;
      }  
      
      return null;
    }
    private ApplicationDirectory _applicationDirectory;
    private X509Certificate _certificate;
    private List<RemoteApplicationHistory> _history = new List<RemoteApplicationHistory>(HistoryMaxLength);
    private int _httpAppicationRootLength;
    private object _thisLock = new object();
    private bool _isSoap = false;
    private PvpTokenHandling _pvpInformationHandling;
    private bool _logTraffic;
    private bool _isolateCookies;
    private List<string> _passThroughCookies;
    
    public RemoteApplication(ApplicationDirectory applicationDirectory)
    {
      lock (_thisLock)
      {
        _applicationDirectory = applicationDirectory;

        if (_applicationDirectory.CertificateFile != null)
        {
          string filename = _applicationDirectory.CertificateFile;
          if (!(filename.StartsWith(@"\") || (filename.Length > 2 && filename.Substring(1, 2) == @":\")))
          {
            filename = HttpContext.Current.Server.MapPath(filename);
          }

          _certificate = new X509Certificate(filename);
        }

        TraceScope.Current.TraceData(TraceEventType.Start, (int) Event.CreateRemoteApplication,
                                    String.Format("RemoteApplication created for RootUrl={0}",
                                                  _applicationDirectory.FullTargetPath));
        _httpAppicationRootLength = HttpContext.Current.Request.ApplicationPath.Length;

        _pvpInformationHandling = applicationDirectory.PvpInformationHandling;
        _logTraffic = applicationDirectory.LogTraffic;
        _isolateCookies = applicationDirectory.IsolateCookies;
        _passThroughCookies = string.IsNullOrEmpty(applicationDirectory.PassThroughCookies) ? new List<string> { } : new List<string> (applicationDirectory.PassThroughCookies.Split(" ".ToCharArray()));
      }
    }

    public ApplicationDirectory Directory
    {
      get { return _applicationDirectory; }
    }

    public PvpTokenHandling PvpInformationHandling
    {
      get { return _pvpInformationHandling; }
    }

    public List<string> PassThroughCookies
    {
      get { return _passThroughCookies; }
    }
    public bool IsolateCookies
    {
      get { return _isolateCookies; }
    }

    public bool LogTraffic
    {
      get { return _logTraffic; }
    }

    public virtual string RemoteApplicationProxyPath
    {
      get
      {
        string path = HttpContext.Current.Request.ApplicationPath + "/" + _applicationDirectory.FullPath + "/";
        while (path.Contains("//"))
          path = path.Replace("//", "/");
        return path;
      }
    }

    public virtual string RootUrl
    {
      get { return _applicationDirectory.RootUrl; }
    }

    public virtual X509Certificate Certificate
    {
      get { return _certificate; }
    }

    //protected virtual string LeftSideUrlTail (string leftSideUrlPart)
    //{
    //  if (!leftSideUrlPart.StartsWith (_path))
    //    throw new ApplicationException ("Url does not start with Path of this Application.");

    //  return leftSideUrlPart.Substring (_path.Length);
    //}

    public virtual HttpWebRequest CreateRightSideRequest(HttpRequest leftSideRequest, Stream inputBuffer, TrafficLogger logger)
    {
      RequestHandler requestHandler = new RequestHandler(leftSideRequest, this, logger);
      HttpWebRequest rightSideRequest = requestHandler.CreateRightSideRequest(inputBuffer);
      _isSoap = requestHandler.IsSoap;

      //lock (_history)
      //{
      // no problem if history is corrupt, we do not lock
      if (_history.Count >= HistoryMaxLength)
        _history.RemoveAt(0);
      _history.Add(new RemoteApplicationHistory(leftSideRequest, rightSideRequest));
      //}

      return rightSideRequest;
    }

    public bool IsSoap
    {
      get { return _isSoap; }
    }

    public bool ByPass(string url)
    {
      return _applicationDirectory.ByPass(url);
    }

    public bool? SubstituteHostInLocationHeader
    {
      get { return _applicationDirectory.SubstituteHostInLocationHeader; }
    }

    public virtual void ShapeHttpResponse(HttpWebResponse rightSideResponse, HttpResponse leftSideResponse)
    {
      HeaderTransformer headerTransformer =
        new HeaderTransformer(rightSideResponse, leftSideResponse, RootUrl, RemoteApplicationProxyPath, IsolateCookies, SubstituteHostInLocationHeader, _passThroughCookies);
      headerTransformer.Transform();
      
      leftSideResponse.StatusCode = (int) rightSideResponse.StatusCode;
      leftSideResponse.ContentType = rightSideResponse.ContentType;
      leftSideResponse.StatusDescription = rightSideResponse.StatusDescription;
    }

    public virtual string GetRightSideUrl(HttpRequest leftSideRequest)
    {
      string path = leftSideRequest.Path.Substring(_httpAppicationRootLength);
      if (!path.StartsWith("/"))
        path = "/" + path;

      string rightSideUrl = _applicationDirectory.GetFullTargetPath(path + leftSideRequest.Url.Query);

      if (rightSideUrl.EndsWith("//"))
        rightSideUrl = rightSideUrl.Substring(0, rightSideUrl.Length - 1);

      var urlSessionId = leftSideRequest.Headers["AspFilterSessionId"];
      if (urlSessionId != null && urlSessionId.StartsWith("S(") && urlSessionId.EndsWith(")"))
      {
        var insertIndex = _applicationDirectory.RootUrl.Length - 1;
        rightSideUrl = rightSideUrl.Insert(insertIndex, "/(" + urlSessionId + ")");
      }

      TraceScope.Current.TraceData(TraceEventType.Verbose, (int) Event.CalculateRightSideUrl,
                                  String.Format("LeftSideUrl={0} RigthSideUrl={1}", path, rightSideUrl));

      return rightSideUrl;
    }

    public List<RemoteApplicationHistory> History
    {
      get { return _history; }
    }
  }
}