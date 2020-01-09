/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Egora.Stammportal.LdapAuthorizationService.Properties;

namespace Egora.Stammportal.LdapAuthorizationService
{
  [XmlRoot(ElementName = "Configuration", Namespace = LdapConfiguration.Namespace, IsNullable = false)]
  public class LdapConfiguration
  {
    public const string Namespace = "http://www.egora.at/Stammportal/LdapConfiguration/1.1";

    private static Dictionary<string, LdapConfiguration> s_configuration = new Dictionary<string, LdapConfiguration>(StringComparer.InvariantCultureIgnoreCase);
    private static object s_configurationLock = new object();

    public static LdapConfiguration GetConfiguration(string configFileName, bool reload)
    {
      if (!s_configuration.ContainsKey(configFileName) || reload)
      {
        lock (s_configurationLock)
        {
          XmlSerializer serializer = new XmlSerializer(typeof (LdapConfiguration));
          using (StreamReader f = File.OpenText(configFileName))
          {
            var ldapConfiguration = (LdapConfiguration) serializer.Deserialize(f);
            var global = ldapConfiguration.GlobalApplication;
            foreach (var application in ldapConfiguration.Applications)
            {
              application.GlobalApplication = global;
            }

            s_configuration.Add(configFileName,  ldapConfiguration);
          }
        }
      }
      return s_configuration[configFileName];
    }

    public static LdapConfiguration GetConfiguration()
    {
      return GetConfiguration(Properties.Settings.Default.ConfigFile, false);
    }
    public static LdapConfiguration GetConfiguration(string configFileName)
    {
      return GetConfiguration(configFileName, false);
    }
    public static LdapConfiguration GetConfiguration(bool reload)
    {
      return GetConfiguration(Properties.Settings.Default.ConfigFile, reload);
    }

    public static string DefaultConfigFile
    {
      get { return Settings.Default.ConfigFile; }
    }
    public LdapConfiguration()
    {
    }

    private ApplicationConfiguration[] _applications;

    [XmlElement("Application")]
    public ApplicationConfiguration[] Applications
    {
      get { return _applications; }
      set { _applications = value; }
    }

    [XmlIgnore]
    public ApplicationConfiguration GlobalApplication
    {
      get
      {
        foreach (ApplicationConfiguration app in Applications)
        {
          if (app.Name == "Global")
            return app;
        }
        return null;
      }
    }

    public ApplicationConfiguration GetApplication(string rootUrl)
    {
      foreach (ApplicationConfiguration app in Applications)
      {
        if (app.IsWeb(rootUrl) || app.IsSoap(rootUrl) || app.IsSaml(rootUrl))
          return app;
      }
      return null;
    }
  }
}