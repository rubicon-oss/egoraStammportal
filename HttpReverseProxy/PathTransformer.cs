/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Web;
using Egora.Stammportal.HttpReverseProxy.Properties;

namespace Egora.Stammportal.HttpReverseProxy
{
  public class PathTransformer
  {
    public PathTransformer(string targetRootUrl, string remoteApplicationProxyPath)
    {
      if (targetRootUrl == null)
        throw new ArgumentException("targetUrlBasePath must not be null.");
      if (remoteApplicationProxyPath == null)
        throw new ArgumentException("remoteApplicationProxyPath must not be null.");

      _targetRootUrl = targetRootUrl;
      _remoteApplicationProxyPath = remoteApplicationProxyPath;
      _targetBasePath = new Uri(targetRootUrl).AbsolutePath;
    }

    private string _remoteApplicationProxyPath;
    private string _targetRootUrl;
    private string _targetBasePath;

    public virtual string AdjustPath(string rightSidePath)
    {
      if (rightSidePath == null)
        return null;

      string leftSidePath = rightSidePath;

      if (leftSidePath.StartsWith("/") || leftSidePath.StartsWith("http://") ||
          leftSidePath.StartsWith("https://"))
      {
        if (leftSidePath.StartsWith(_targetRootUrl, StringComparison.OrdinalIgnoreCase))
        {
          leftSidePath = _remoteApplicationProxyPath + leftSidePath.Substring(_targetRootUrl.Length);
        }
        else if (leftSidePath.StartsWith(_targetBasePath, StringComparison.OrdinalIgnoreCase))
        {
          leftSidePath = _remoteApplicationProxyPath + leftSidePath.Substring(_targetBasePath.Length);
        }
        else if(Settings.Default.SubstituteHostInLocationHeader)
        {
          Uri rightSideUri = new Uri(leftSidePath);
          if (! (rightSideUri.Authority == HttpContext.Current.Request.Url.Authority && rightSideUri.Scheme == HttpContext.Current.Request.Url.Scheme) )
          {
            Uri contextUrl = HttpContext.Current.Request.Url;
            UriBuilder uriBuilder = new UriBuilder(leftSidePath);
            uriBuilder.Host = contextUrl.Host;
            uriBuilder.Port = contextUrl.Port;
            uriBuilder.Scheme = contextUrl.Scheme;
            leftSidePath = uriBuilder.ToString();
          }
        }

        string[] leftSideParts = leftSidePath.Split(new string[] { "://" }, 2, StringSplitOptions.None);
        if (leftSideParts.Length == 1)
          leftSidePath = leftSidePath.Replace("//", "/");
        else if (leftSideParts.Length == 2)
          leftSidePath = leftSideParts[0] + "://" + leftSideParts[1].Replace("//", "/");
      }

      return leftSidePath;
    }
  }
}