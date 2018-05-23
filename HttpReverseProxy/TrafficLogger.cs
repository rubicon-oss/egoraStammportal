using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Egora.Stammportal.HttpReverseProxy.Properties;

namespace Egora.Stammportal.HttpReverseProxy
{
  public class TrafficLogger
  {
    private const string _logSeparator = "----------------------";
    private string _logFileDir;
    private TraceScope _traceScope;

    public TrafficLogger(string remoteApplicationProxyPath, string userId, TraceScope traceScope, Uri url)
    {
      StringBuilder p = new StringBuilder(remoteApplicationProxyPath);
      p.Replace('/', '_');
      StringBuilder u = new StringBuilder(userId);
      u.Replace('\\', '_');
      StringBuilder path = new StringBuilder(url.GetComponents(UriComponents.Path, UriFormat.Unescaped));
      path.Replace('/', '_');
      path.Replace('.', '_');

      _logFileDir = Path.Combine(Path.Combine(Path.Combine(Settings.Default.TrafficLogDir, GetValidString(p)), GetValidString(u)),
                                 DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" +  GetValidString(path) + "_" +
                                 Guid.NewGuid());
      _traceScope = traceScope;
    }

    private string GetValidString(StringBuilder p)
    {
      foreach (char i in Path.GetInvalidPathChars())
        p.Replace(i, '_');
      return p.ToString();
    }

    public void LogRequestInformation(HttpWebRequest webRequest)
    {
      string logFileName =  _logFileDir +"\\1_RequestInformation.txt";
      NameValueCollection headers = new NameValueCollection();
      headers.Add("Request Headers", _logSeparator);
      headers.Add(webRequest.Headers);
      LogHeader(headers, logFileName);
      headers = new NameValueCollection();
      headers.Add("other Request Information", _logSeparator);
      headers.Add("Url", webRequest.RequestUri.ToString());
      if (webRequest.CookieContainer != null)
        headers.Add("Cookie", webRequest.CookieContainer.GetCookieHeader(webRequest.RequestUri));
      headers.Add("Host", webRequest.Host);
      headers.Add("Method", webRequest.Method);
      if (webRequest.ClientCertificates != null && webRequest.ClientCertificates.Count > 0)
        headers.Add("ClientCert Subject", webRequest.ClientCertificates[0].Subject);
      LogHeader(headers, logFileName);
      headers = new NameValueCollection();
      headers.Add("Input-Headers", _logSeparator);
      foreach (string k in HttpContext.Current.Request.ServerVariables.AllKeys)
      {
        string value = HttpContext.Current.Request.ServerVariables[k];
        if (k != "ALL_RAW" && k != "ALL_HTTP" && !String.IsNullOrEmpty(value))
          headers.Add(k, value);
      }
      LogHeader(headers, logFileName);
    }

    private void LogHeader(NameValueCollection webHeaderCollection, string logFileName)
    {
      try
      {
        EnsureLogDir();
        StringBuilder h = new StringBuilder();
        foreach (string key in webHeaderCollection.AllKeys)
        {
          h.Append(key).Append(": ").AppendLine(webHeaderCollection[key]);
        }
        File.AppendAllText(logFileName, h.ToString());
      }
      catch (Exception e)
      {
        _traceScope.TraceEvent(TraceEventType.Error, (int) HttpReverseProxyHandler.Event.ProcessingResponse, e.Message);
      }
    }

    public void LogResponseInformation(HttpWebResponse response)
    {
      string logFileName = _logFileDir + "\\3_ResponseHeaders.txt";
      NameValueCollection headers = new NameValueCollection(response.Headers);
      headers.Add("StatusCode", response.StatusCode.ToString("D"));
      headers.Add("StatusDescription", response.StatusDescription); 
      LogHeader(headers, logFileName);
    }

    public void LogResponseContent(MemoryStream output)
    {
      output.Position = 0;
      try
      {
        EnsureLogDir();
        File.WriteAllBytes(_logFileDir + "\\4_Response.txt", output.ToArray());
      }
      catch (Exception e)
      {
        _traceScope.TraceEvent(TraceEventType.Error, (int) HttpReverseProxyHandler.Event.ProcessingResponse, e.Message);
      }
      output.Position = 0;
    }

    private void EnsureLogDir()
    {
      if (!Directory.Exists(_logFileDir))
        Directory.CreateDirectory(_logFileDir);
    }
    public void LogRequestContent(MemoryStream buffer)
    {
      buffer.Position = 0;
      try
      {
        EnsureLogDir();
        File.WriteAllBytes(_logFileDir + "\\2_Request.txt", buffer.ToArray());
      }
      catch (Exception e)
      {
        _traceScope.TraceEvent(TraceEventType.Error, (int)Egora.Stammportal.HttpReverseProxy.HttpReverseProxyHandler.Event.ProcessingResponse, e.Message);
      }
      buffer.Position = 0;
    }

    public void LogRequestTiming(string message)
    {
      try
      {
        EnsureLogDir();
        File.WriteAllText(_logFileDir + "\\5_Timing.txt", message);
      }
      catch (Exception e)
      {
        _traceScope.TraceEvent(TraceEventType.Error, (int)Egora.Stammportal.HttpReverseProxy.HttpReverseProxyHandler.Event.ProcessingResponse, e.Message);
      }
    }

  }
}