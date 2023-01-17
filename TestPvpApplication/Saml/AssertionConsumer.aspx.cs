using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Assertions;

namespace TestPvpApplication.Saml
{
  public partial class AssertionComsumer : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      var m = GetSamlMessage();
      if (m != null)
      {
        bool isInResponseTo;
        string partnerIdP;
        string authnContext;
        string userName;
        SAMLAttribute[] attributes;
        string relayState;

        // Receive and process the SAML assertion contained in the SAML response.
        SAMLServiceProvider.ReceiveSSO(Request, out isInResponseTo, out partnerIdP, out authnContext, out userName, out attributes, out relayState);
        SamlMessageTextBox.Text = m.OuterXml;
        UserNameTextBox.Text = userName;
      }
    }

    private XmlElement GetSamlMessage()
    {
      // only http POST binding implemented
      string form = Request.Form["SAMLResponse"];
      if (form == null)
        return null;

      var message = Encoding.UTF8.GetString(Convert.FromBase64String(form));

      XmlReaderSettings settings = new XmlReaderSettings();
      settings.DtdProcessing = DtdProcessing.Prohibit;
      settings.XmlResolver = null;
      XmlDocument document = new XmlDocument()
      {
        PreserveWhitespace = true,
        XmlResolver = null
      };
      using (XmlReader reader = XmlReader.Create(new StringReader(message), settings))
      {
        document.Load(reader);
      }

      var samlMessage = document.DocumentElement;
      return samlMessage;
    }
  }
}