/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using Egora.Stammportal.HttpReverseProxy.Properties;
using Egora.Stammportal.HttpReverseProxy.StreamFilter;

namespace Egora.Stammportal.HttpReverseProxy
{
  public class HttpReverseProxyHandler : IHttpHandler //, IHttpAsyncHandler // for later
  {
    private static bool s_initialized;
    private static object s_initializeLock = new object();
    private static Egora.Stammportal.HttpReverseProxy.Mapping.PathMap s_map;

    public enum Event
    {
      Initializing = 100,
      ProcessingRequest,
      ProcessingResponse,
      NotFound,
      RetrievingApplications,
      NotAuthorized,
      StreamFilter,
    }

    protected virtual void Initialize()
    {
      using (TraceScope traceScope = new TraceScope(null))
      {
        traceScope.TraceEvent(TraceEventType.Start, (int) Event.Initializing,
                                     "ReverseProxy initializing started.");

        string filename =
          HttpContext.Current.Server.MapPath(
            Egora.Stammportal.HttpReverseProxy.Properties.Settings.Default.PathMapFile);
        lock (s_initializeLock)
        {
          s_map = Egora.Stammportal.HttpReverseProxy.Mapping.PathMap.CreateFromFile(filename);
          RemoteApplication.Initialize(s_map);

          ServicePointManager.MaxServicePointIdleTime = Settings.Default.ConnectionMaxIdleTimeSeconds*1000;
          ServicePointManager.DefaultConnectionLimit = Settings.Default.ConnectionsPerServer;
          ServicePointManager.SecurityProtocol = (SecurityProtocolType) Settings.Default.SecurityProtocol;
          //SecurityProtocolType.Ssl3=48
          //SecurityProtocolType.Tls=192;
          //SecurityProtocolType.Tls11=768
          //SecurityProtocolType.Tls12=3072

          s_initialized = true;
        }

        traceScope.TraceEvent(TraceEventType.Start, (int) Event.Initializing,
                                     "ReverseProxy initializing finished.");
      }
    }

    #region IHttpHandler Members

    bool IHttpHandler.IsReusable
    {
      get { return false; }
    }

    void IHttpHandler.ProcessRequest(HttpContext context)
    {
      using (TraceScope traceScope = new TraceScope(context))
      {
        traceScope.TraceEvent(TraceEventType.Verbose, (int) Event.ProcessingRequest,
          "Processing request started.");
        if (context.Request == null)
        {
          //happens when application starts first time
          traceScope.TraceEvent(TraceEventType.Verbose, (int) Event.ProcessingRequest, "Context is null.");
          return;
        }

        using (var executionScope = new ExecutionScope(nameof(IHttpHandler.ProcessRequest)))
        {
          if (!s_initialized)
            Initialize();

          Authentication auth = new Authentication(context.Request);
          if (ProcessUiPage(context, auth))
            return;

          RemoteApplication remoteApplication = RemoteApplication.GetRemoteApplication(context.Request);

          if (remoteApplication == null)
          {
            traceScope.TraceEvent(TraceEventType.Warning, (int) Event.NotFound,
              "No RemoteApplication found. Ending with 404.");
            context.Response.StatusCode = 404;
            context.Response.End();
          }

          TrafficLogger logger = GetLogger(context, remoteApplication, traceScope);
          executionScope.SetLogger(logger);

          HttpWebRequest webRequest = null;
          HttpWebResponse webResponse = null;

          using (Stream inputBuffer = CopyFilter.GetInputStream(context.Request))
          {
            for (int tries = 0; tries <= Properties.Settings.Default.NetworkRetryCount; tries++)
            {
              if (tries > 0)
              {
                System.Threading.Thread.Sleep(Properties.Settings.Default.NetworkRetryDelay);
                traceScope.TraceEvent(TraceEventType.Error, (int) Event.ProcessingResponse,
                  "trying again.");
                inputBuffer.Position = 0;
              }

              using (new ExecutionScope(nameof(remoteApplication.CreateRightSideRequest)))
              {
                try
                {
                  webRequest = remoteApplication.CreateRightSideRequest(context.Request, inputBuffer, logger);

                  if (remoteApplication.LogTraffic)
                    logger.LogRequestInformation(webRequest);
                }
                catch (AuthorizationException e)
                {
                  traceScope.TraceEvent(TraceEventType.Verbose, (int) Event.NotAuthorized, e.Message);
                  context.Response.StatusCode = 406;
                  context.Response.End();
                }
              }

              traceScope.TraceEvent(TraceEventType.Verbose, (int) Event.ProcessingRequest,
                "Processing request finished.");

              using (new ExecutionScope(nameof(auth.GetWebResponse)))
              {
                try
                {
                  webResponse = auth.GetWebResponse(webRequest, traceScope);
                }
                catch (WebException ex)
                {
                  traceScope.TraceEvent(TraceEventType.Error, (int)Event.ProcessingResponse,
                    "Could not get right side response: {0}", ex.Message);
                  if (ex.Response == null)
                  {
                    if (IsRetryable(ex, remoteApplication.IsSoap))
                    {
                      if (webRequest != null)
                      {
                        webRequest.Abort();
                        webRequest = null;
                      }
                      continue;
                    }
                    else if (ex.Status == WebExceptionStatus.Timeout)
                    {
                      traceScope.TraceEvent(TraceEventType.Error, (int)Event.ProcessingResponse,
                        "Ending with 408 Timeout.");
                      context.Response.StatusCode = 408;
                      context.Response.End();
                      /*
                    408 Request Timeout Der Server hat eine erwartete Anfrage nicht innerhalb des dafür festgelegten Maximalzeitraums erhalten. Die Verbindung zum anfragenden Browser wird deshalb abgebaut. Angeforderte Daten werden nicht übertragen. 
                    412 Precondition Failed Eine oder mehrere Bedingungen, die bei der Anfrage gestellt wurden, treffen nicht zu. Die angeforderten Daten werden deshalb nicht übertragen. 
                    500 Internal Server Error Der Server kann die angeforderten Daten nicht senden, weil auf dem Server ein Fehler aufgetreten ist. Beispielsweise konnte das aufgerufene CGI-Script nicht gestartet werden. 
                    502 Bad Gateway Zum Bearbeiten der Anfrage musste der Server einen anderen Server aufrufen, erhielt dabei jedoch eine Fehlermeldung. Die angeforderten Daten können deshalb nicht gesendet werden. 
                    503 Service Unavailable Der Server kann die Anfrage wegen Überlastung nicht bearbeiten. Die angeforderten Daten können deshalb nicht gesendet werden. In der Statusmeldung kann stehen, wann die Anfrage frühestens wieder bearbeitet werden kann. Im Gegensatz zum Status-Code 202 verarbeitet der Server die Daten nicht, sobald er wieder Kapazitäten hat. 
                    504 Gateway Timeout Zum Bearbeiten der Anfrage musste der Server einen anderen Server aufrufen, erhielt dabei jedoch nach einem festgelegten Maximalzeitraum keine Antwort. Die angeforderten Daten können deshalb nicht gesendet werden. 
                    */
                    }
                    else
                    {
                      traceScope.TraceEvent(TraceEventType.Error, (int)Event.ProcessingResponse, "Ending with 500.");
                      traceScope.TraceEvent(TraceEventType.Error, (int)Event.ProcessingResponse,
                        "No response available.");
                      traceScope.TraceEvent(TraceEventType.Error, (int)Event.ProcessingResponse,
                        "Input stream follows.");
                      LogInputStream(context, inputBuffer, traceScope);
                      context.Response.StatusCode = 500;
                      context.Response.StatusDescription = ex.Message;
                      context.Response.End();
                    }
                  }
                  else
                  {
                    webResponse = (HttpWebResponse)ex.Response;
                  }
                } // end catch webResponse = auth.GetWebResponse (webRequest);
              }

              if (ShouldRetry(webResponse, remoteApplication.IsSoap))
              {
                traceScope.TraceEvent(TraceEventType.Error, (int) Event.ProcessingResponse,
                  "Received " + webResponse.StatusCode);
                if (tries < Properties.Settings.Default.NetworkRetryCount)
                  webResponse.Close();
                webRequest = null;
                continue;
              }
              break;
            } // end for tries
          } // end using inputBuffer

          if (webResponse == null)
          {
            traceScope.TraceEvent(TraceEventType.Verbose, (int) Event.ProcessingResponse,
              "Response is null.");
            context.Response.StatusCode = 502;
            context.Response.StatusDescription = "No response reveived from upstream server.";
            context.Response.End();
          }

          bool errorEncountered = webResponse.StatusCode.Equals(HttpStatusCode.InternalServerError);
          bool bufferResponse = Settings.Default.BufferRightSide || errorEncountered || remoteApplication.LogTraffic;
          using (Stream output = GetOutputStream(context.Response, webResponse.ContentLength, bufferResponse))
          {
            using (HttpWebResponse response = webResponse)
            {
              traceScope.TraceEvent(TraceEventType.Verbose, (int) Event.ProcessingResponse,
                "Processing response started.");
              if (remoteApplication.LogTraffic)
              {
                logger.LogResponseInformation(response);
              }

              remoteApplication.ShapeHttpResponse(response, context.Response);

              Stream rightSideResponseStream = response.GetResponseStream();

              CopyFilter filter = new CopyFilter(response.ContentLength);
              filter.FilterStream(rightSideResponseStream, output);

              traceScope.TraceEvent(TraceEventType.Verbose, (int) Event.ProcessingResponse,
                "Processing response finished.");
            }

            if (errorEncountered)
            {
              output.Position = 0;
              byte[] buffer = new byte[output.Length];
              output.Read(buffer, 0, (int) output.Length);
              Encoding encoding = Encoding.UTF8;
              if (context.Response.ContentEncoding != null)
                encoding = context.Response.ContentEncoding;
              traceScope.TraceEvent(TraceEventType.Error, (int) Event.ProcessingResponse,
                "Response stream follows.");
              traceScope.TraceData(TraceEventType.Error, (int) Event.ProcessingResponse,
                encoding.GetString(buffer));
            }

            if (remoteApplication.LogTraffic)
            {
              logger.LogResponseContent((MemoryStream) output);
            }

            if (bufferResponse)
            {
              output.Position = 0;
              CopyFilter filter = new CopyFilter(output.Length);
              filter.FilterStream(output, context.Response.OutputStream);
            }

            context.Response.End();
          }
        }
      }
    }

    private static void LogInputStream(HttpContext context, Stream inputBuffer, TraceScope traceScope)
    {
      try
      {
        inputBuffer.Position = 0;
        byte[] buffer = new byte[inputBuffer.Length];
        inputBuffer.Read(buffer, 0, (int) inputBuffer.Length);
        Encoding encoding = Encoding.UTF8;
        if (context.Request.ContentEncoding != null)
          encoding = context.Request.ContentEncoding;
        traceScope.TraceData(TraceEventType.Error, (int) Event.ProcessingResponse,
                                    encoding.GetString(buffer));
      }
      catch
      {
      }
    }

    private TrafficLogger GetLogger(HttpContext context, RemoteApplication remoteApplication, TraceScope traceScope)
    {
      if (remoteApplication.LogTraffic)
      {
        Authentication auth = new Authentication(context.Request);
        TrafficLogger logger = new TrafficLogger(remoteApplication.RemoteApplicationProxyPath, auth.UserId, traceScope, context.Request.Url);
        return logger;
      }
      return null;
    }

    private bool ProcessUiPage(HttpContext context, Authentication auth)
    {
      string path = context.Request.Path.ToLowerInvariant();
      string adminpath = (context.Request.ApplicationPath + Settings.Default.AdministrationPath + "/").ToLowerInvariant();
      if (path.StartsWith(adminpath))
      {
        if (auth.IsAdmin)
        {
          if (path.Replace(adminpath, "") == "reset.aspx")
            Initialize();
          string pagePath = path.Replace(adminpath, context.Request.ApplicationPath + "/Administration/");
          IHttpHandler pageHandler =
            PageParser.GetCompiledPageInstance(path, context.Request.MapPath(pagePath), context);
          pageHandler.ProcessRequest(context);
          return true;
        }
      }
      return false;
    }

    private Stream GetOutputStream(HttpResponse response, long bufferSize, bool useMemoryStream)
    {
      Stream output;
      int size = bufferSize > 0 ? (int) bufferSize : 1024;
      if (useMemoryStream)
      {
        output = new MemoryStream(size);
      }
      else
      {
        output = response.OutputStream;
      }
      return output;
    }

    private static string[] s_retryableExceptionMessages =
      Properties.Settings.Default.RetryableErrorMessages.Split(';');

    private bool IsRetryable(WebException ex, bool isSoap)
    {
      if (isSoap && !Settings.Default.RetrySoap)
        return false;

      foreach (string m in s_retryableExceptionMessages)
      {
        if (ex.Message.Contains(m))
          return true;
      }
      return false;
    }

    private static string[] s_retryableHosts = Properties.Settings.Default.RetryableHosts.Split(';');

    private bool ShouldRetry(HttpWebResponse webResponse, bool isSoap)
    {
      if (isSoap && !Settings.Default.RetrySoap)
        return false;

      if (webResponse == null)
        return false;

      bool retry = webResponse.StatusCode == HttpStatusCode.InternalServerError;

      if (!retry)
        return false;

      string url = webResponse.ResponseUri.AbsoluteUri.ToLowerInvariant();
      foreach (string host in s_retryableHosts)
      {
        if (url.Contains(host))
          return retry;
      }

      return false;
    }

    #endregion

    #region IHttpAsyncHandler Members

    //IAsyncResult IHttpAsyncHandler.BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
    //{
    //  throw new Exception("The method or operation is not implemented.");
    //}

    //void IHttpAsyncHandler.EndProcessRequest(IAsyncResult result)
    //{
    //  throw new Exception("The method or operation is not implemented.");
    //}

    #endregion
  }
}