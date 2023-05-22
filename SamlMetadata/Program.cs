using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ComponentSpace.SAML2.Configuration;
using ComponentSpace.SAML2.Metadata;

namespace SamlMetadata
{
  public class Program
  {
    static int Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine("usage: SamlMetadata.exe import|export");
        return 1;
      }

      if (args[0].Equals("e", StringComparison.InvariantCultureIgnoreCase)   || args[0].Equals("export", StringComparison.InvariantCultureIgnoreCase))
      {
        SAMLConfiguration samlConfiguration = LoadSAMLConfiguration();

        if (samlConfiguration.LocalIdentityProviderConfiguration == null)
          throw new ApplicationException($"Export requested, but no LocalIdentityProviderConfiguration found");

        var entityDescriptor = ExportIdentityProviderMetadata(samlConfiguration);

        SaveMetadata(entityDescriptor);
        
        return 0;
      }

      if (args[0].Equals("i", StringComparison.InvariantCultureIgnoreCase) || args[0].Equals("import", StringComparison.InvariantCultureIgnoreCase))
      {
        SAMLConfiguration samlConfiguration = LoadSAMLConfiguration();
        
        if (samlConfiguration.LocalIdentityProviderConfiguration == null)
          throw new ApplicationException($"Import requested, but no LocalIdentityProviderConfiguration found");

        ImportMetadata(samlConfiguration);
        
        return 0;
      }
 
      return 1;
    }

    private static void ImportMetadata(SAMLConfiguration samlConfiguration)
    {
      var metaData = new XmlDocument();
      metaData.PreserveWhitespace = true;
      var metadataFilename = Properties.Settings.Default.MetadataFilename;
      if (!File.Exists(metadataFilename))
        throw new ApplicationException($"Metadata File '{metadataFilename}' not found.");

      metaData.Load(metadataFilename);
      var esd = new EntitiesDescriptor(metaData.DocumentElement);
      MetadataImporter.ImportServiceProviders(esd, samlConfiguration, "certificates");
      
      var newSamlConfig = samlConfiguration.ToXml().OwnerDocument;

      var fileName = Properties.Settings.Default.SamlConfigFilename;
      using (XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, Encoding.UTF8))
      {
        xmlTextWriter.Formatting = Formatting.Indented;
        newSamlConfig.Save(xmlTextWriter);
      }
    }

    private static SAMLConfiguration LoadSAMLConfiguration()
    {
      var fileName = Properties.Settings.Default.SamlConfigFilename;

      if (!File.Exists(fileName))
      {
        throw new ApplicationException($"The configuration file '{fileName}' doesn't exist.");
      }

      SAMLConfigurations samlConfigurations;
      try
      {
        samlConfigurations = SAMLConfigurationFile.Load(fileName);
      }
      catch (ComponentSpace.SAML2.Exceptions.SAMLSchemaValidationException e)
      {
        var violations = e.Warnings?.Count > 0 ? string.Join(Environment.NewLine, e.Warnings.Select(w => w.Message)) : string.Empty;
        throw new ApplicationException(violations, e);
      }

      if (samlConfigurations.Configurations.Count == 1)
      {
        return samlConfigurations.Configurations[0];
      }

      var configurationId = Properties.Settings.Default.ConfigurationName;

      return samlConfigurations.GetConfiguration(configurationId);
    }
    private static X509Certificate2 LoadCertificate(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
      {
        return null;
      }

      if (!File.Exists(fileName))
      {
        throw new ArgumentException(string.Format("The X.509 certificate file {0} doesn't exist.", fileName));
      }

      return new X509Certificate2(fileName);
    }
    private static EntityDescriptor ExportIdentityProviderMetadata(SAMLConfiguration samlConfiguration)
    {
      if (samlConfiguration.LocalIdentityProviderConfiguration == null)
        throw new ApplicationException($"No LocalIdentityProviderConfiguration found.");

      var lidp = samlConfiguration.LocalIdentityProviderConfiguration;
      var signingCertConfig = lidp.LocalCertificates.FirstOrDefault(c => c.Use == CertificateConfiguration.CertificateUse.Any);

      X509Certificate2 x509Certificate = null;
      if (signingCertConfig != null)
      {
        if (!string.IsNullOrEmpty(signingCertConfig.Thumbprint))
        {
          using (X509Store store = new X509Store(signingCertConfig.StoreName, signingCertConfig.StoreLocation))
          {
            try
            {
              store.Open(OpenFlags.ReadOnly);
              var results = store.Certificates.Find(X509FindType.FindByThumbprint, signingCertConfig.Thumbprint, true);
              if (results == null || results.Count == 0)
                throw new ApplicationException(
                  $"Certificate with thumbprint {signingCertConfig.Thumbprint} not found in localmachine/my.");

              x509Certificate = results[0];
            }
            finally
            {
              store.Close();
            }
          }
        }
      }

      return MetadataExporter.Export(samlConfiguration, x509Certificate, lidp.SingleSignOnServiceUrl, lidp.SingleLogoutServiceUrl, null);
    }
    private static void SaveMetadata(EntityDescriptor entityDescriptor)
    {
      string fileName = Properties.Settings.Default.MetadataFilename;

      using (XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, null))
      {
        xmlTextWriter.Formatting = Formatting.Indented;
        entityDescriptor.ToXml().OwnerDocument.Save(xmlTextWriter);
      }
    }
  }
}
