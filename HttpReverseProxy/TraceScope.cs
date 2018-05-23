using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Egora.Stammportal.HttpReverseProxy
{
  [Switch("reverseProxySwitch", typeof(SourceSwitch))]
  public class TraceScope : IDisposable
  {

    internal static TraceSource Trace = new TraceSource("ReverseProxy");
    private static object HttpContextKey = new object();

    public static TraceScope Current
    {
      get
      {
        if (HttpContext.Current==null)
          return new TraceScope(null);

        TraceScope current = (TraceScope)HttpContext.Current.Items[HttpContextKey];
        if (current == null)
          return new TraceScope(HttpContext.Current);
        
        return current;
      }
    }
    
    public TraceScope(HttpContext currentContext)
    {
      _traceContext = Guid.NewGuid();
      
      if (currentContext != null)
        currentContext.Items[HttpContextKey] = this;
    }

    
    private bool disposed = false;
    private Guid _traceContext;
    private bool _hasError = false;

    private List<TraceInfo> _traceInfos = new List<TraceInfo>(); 
    public void TraceEvent(TraceEventType type, int eventId, string format, params object[] args)
    {
      _hasError = _hasError || type.Equals(TraceEventType.Error) || type.Equals(TraceEventType.Critical);
      _traceInfos.Add(new TraceInfo() {Type = type, EventId = eventId, Format = format, Args = args});
    }

    public void TraceData(TraceEventType type, int eventId, string data)
    {
      _hasError = _hasError || type.Equals(TraceEventType.Error) || type.Equals(TraceEventType.Critical);
      _traceInfos.Add(new TraceInfo() { Type = type, EventId = eventId, Format = "{0}", Args = new object[] { data } });
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern. 
    protected virtual void Dispose(bool disposing)
    {
      if (disposed)
        return;

      if (disposing)
      {
        TraceEventType? type = _hasError ? TraceEventType.Error : (TraceEventType?) null;
        foreach (TraceInfo info in _traceInfos)
        {
          Trace.TraceEvent(type.HasValue ? type.Value : info.Type, info.EventId, _traceContext.ToString() + " " + info.Format, info.Args);
        }
      }

      disposed = true;
    }
  }

  class TraceInfo
  {
    public TraceEventType Type;
    public int EventId;
    public string Format;
    public object[] Args;
  }
}