using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Claims;
using System.Text;
using log4net;
using log4net.Config;

namespace AuthenticationChecker
{
  public class ADHelper
  {
    private static ILog s_log = LogManager.GetLogger(typeof(ADHelper));

    private static DirectoryEntry FindUser(string filter)
    {
      s_log.Info($"searching with filter '{filter}',");
      using (var seacher = new DirectorySearcher(filter))
      {
        seacher.SearchScope = SearchScope.Subtree;
        using (var result = seacher.FindAll())
        {
          s_log.Info($"Found {result.Count} user with filter '{filter}'");
          if (result.Count == 0)
          {
            return null;
          }

          if (result.Count > 1)
          {
            s_log.Error($"Found {result.Count} hits with filter '{filter}'");
            throw new ApplicationException($"More then one user found with filter '{filter}'.");
          }

          var user = result[0].GetDirectoryEntry();
          s_log.Info($"User '{user.Name}' found.");
          return user;
        }
      }
    }

    public string GetFilter(string additionalFilter)
    {
      return $"(&({"objectCategory=user"})({"!userAccountControl:1.2.840.113556.1.4.803:=2"})({additionalFilter}))";
    }

    public string GetSamAccountName(string firstName, string lastName)
    {
      using (var user = FindUser(firstName, lastName))
      {
        if (user != null)
          return user.Properties["samaccountname"][0].ToString();

        return null;
      }
    }

    public DirectoryEntry FindUser(string firstName, string lastName)
    {
      string filter = GetFilter($"(&(givenName={Escape(firstName)})(sn={Escape(lastName)}))");
      var user = FindUser(filter);
      return user;
    }

    public DirectoryEntry FindUser(string firstName, string lastName, string samAccountName)
    {
      string filter = GetFilter($"(&(givenName={Escape(firstName)})(sn={Escape(lastName)})(sAMAccountName={Escape(samAccountName)}))");
      var user = FindUser(filter);
      return user;
    }
    public string Escape(string key)
    {
      if (key == null)
        return null;

      return key.Replace(@"\", @"\5C")
                .Replace("*", @"\2A")
                .Replace("(", @"\28")
                .Replace(")", @"\29")
                .Replace("\0", @"\00");
    }
  }
}
