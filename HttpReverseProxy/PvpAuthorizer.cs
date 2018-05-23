/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/


using System.Web.Services.Protocols;

namespace Egora.Stammportal.HttpReverseProxy
{
  /// <remarks/>
  [System.ComponentModel.DesignerCategory("code")]
  [System.Web.Services.WebServiceBinding(
    Name = "PvpAuthorizerSoap",
    Namespace = CustomAuthorization.Namespace)]
  public partial class PvpAuthorizer : SoapHttpClientProtocol
  {
    private bool useDefaultCredentialsSetExplicitly;

    /// <remarks/>
    public PvpAuthorizer()
    {
      this.Url =
        global::Egora.Stammportal.HttpReverseProxy.Properties.Settings.Default.AuthorizationWebServiceDefault;
      if ((this.IsLocalFileSystemWebService(this.Url) == true))
      {
        this.UseDefaultCredentials = true;
        this.useDefaultCredentialsSetExplicitly = false;
      }
      else
      {
        this.useDefaultCredentialsSetExplicitly = true;
      }
    }

    public new string Url
    {
      get { return base.Url; }
      set
      {
        if ((((this.IsLocalFileSystemWebService(base.Url) == true)
              && (this.useDefaultCredentialsSetExplicitly == false))
             && (this.IsLocalFileSystemWebService(value) == false)))
        {
          base.UseDefaultCredentials = false;
        }
        base.Url = value;
      }
    }

    public new bool UseDefaultCredentials
    {
      get { return base.UseDefaultCredentials; }
      set
      {
        base.UseDefaultCredentials = value;
        this.useDefaultCredentialsSetExplicitly = true;
      }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethod(
      CustomAuthorization.Namespace + "/GetAuthorization",
      RequestNamespace = CustomAuthorization.Namespace,
      ResponseNamespace = CustomAuthorization.Namespace,
      Use = System.Web.Services.Description.SoapBindingUse.Literal,
      ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public CustomAuthorization GetAuthorization(string rootUrl, string userId)
    {
      object[] results = this.Invoke("GetAuthorization", new object[]
                                                           {
                                                             rootUrl,
                                                             userId
                                                           });
      return ((CustomAuthorization) (results[0]));
    }


    private bool IsLocalFileSystemWebService(string url)
    {
      if (((url == null) || (url == string.Empty)))
      {
        return false;
      }

      System.Uri wsUri = new System.Uri(url);
      if (((wsUri.Port >= 1024)
           && (string.Compare(wsUri.Host, "localhost", System.StringComparison.OrdinalIgnoreCase) == 0)))
      {
        return true;
      }

      return false;
    }
  }
}