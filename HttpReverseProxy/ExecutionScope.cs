using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Egora.Stammportal.HttpReverseProxy
{
  public class ExecutionScope : IDisposable
  {

    private static readonly object HttpContextKeyRoot = new object();

    private static ExecutionScope RootExecutionScope
    {
      get
      {
        if (HttpContext.Current == null)
          return null;

        return (ExecutionScope)HttpContext.Current.Items[HttpContextKeyRoot];
      }
    }

    public ExecutionScope(string tag)
    {      
      _tag = tag;
      _startTime = DateTime.Now;

      if (RootExecutionScope==null && HttpContext.Current!=null)
        HttpContext.Current.Items[HttpContextKeyRoot]=this;

      IncreaseLevel();
      WriteStart(tag);
    }

    private bool disposed = false;
    private StringBuilder _message = new StringBuilder();
    private string _tag;
    private DateTime _startTime;
    private int _level = 0;
    private TrafficLogger _logger;

    public void SetLogger(TrafficLogger logger)
    {
      if (RootExecutionScope != null)
        RootExecutionScope._logger = logger;
    }

    private bool IAmRoot => RootExecutionScope == this;

    private void WriteStart(string tag)
    {
      if (IAmRoot)
        _message.AppendLine(String.Format("{0:O}\t{1:000}\t{2}\tStart", DateTime.Now, _level, tag)); 
      else if (RootExecutionScope != null)
        RootExecutionScope.WriteStart(tag);
    }

    private void WriteExit(string tag, DateTime startTime)
    {
      if (IAmRoot)
        _message.AppendLine(String.Format("{0:O}\t{1:000}\t{2}\tExit\t{3:000000}ms", DateTime.Now, _level, tag, (int) (DateTime.Now - _startTime).TotalMilliseconds));
      else if (RootExecutionScope != null)
        RootExecutionScope.WriteExit(tag, startTime);
    }

    private void IncreaseLevel()
    {
      if (RootExecutionScope != null)
        RootExecutionScope._level++;
    }

    private void DecreaseLevel()
    {
      if (RootExecutionScope != null)
        RootExecutionScope._level--;
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
        WriteExit(_tag, _startTime);
        DecreaseLevel();

        if (IAmRoot && _logger!=null)
          _logger.LogRequestTiming(_message.ToString());
      }

      disposed = true;
    }
  }
}