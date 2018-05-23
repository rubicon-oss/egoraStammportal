using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp
{
  public class PvpException : Exception
  {
    public PvpException(string message) : base(message)
    {
      
    }
  }

  public class PvpVersionNotSupportedException : PvpException
  {
    private readonly string _versionInfo;

    public PvpVersionNotSupportedException(string message, string versionInfo)
      : base(message)
    {
      _versionInfo = versionInfo;
    }

    public string VersionInfo
    {
      get { return _versionInfo; }
    }
  }

  public class PvpInitializationException : PvpException
  {
    public PvpInitializationException(string message)
      : base(message)
    {
    }
  }
}
