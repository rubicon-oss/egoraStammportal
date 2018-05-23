/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Egora.Pvp;
using Egora.Stammportal.HttpReverseProxy.Mapping;

namespace Egora.Stammportal.HttpReverseProxy.StreamFilter
{
  public class SoapFilter : CopyFilter
  {
    public SoapFilter(CustomAuthorization soapAuthorization, long streamLength, PvpTokenHandling tokenHandling, NameValueCollection headers)
      : base(streamLength)
    {
      _soapAuthorization = soapAuthorization;
      _tokenHandling = tokenHandling;
      _headers = headers;
    }

    private CustomAuthorization _soapAuthorization;
    private PvpTokenHandling _tokenHandling;
    private NameValueCollection _headers;

    private const string _headerTag = "Header";
    private const string _securityNS = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
    private const string _securityNSLegacy = "http://schemas.xmlsoap.org/ws/2002/04/secext";
    private const string _securityTag = "Security";
    private const string _pvpTokenNS = "http://egov.gv.at/pvp1.xsd";
    private const string _pvpTokenTag = "pvpToken";
    private const string _pvpChainedTokenTag = "pvpChainedToken";

    public override long FilterStream(Stream fromStream, Stream toStream)
    {
      StreamReader reader = new StreamReader(fromStream, System.Text.Encoding.UTF8);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(reader);
      XmlElement insertedToken = InsertAuthorization(xmlDocument, _soapAuthorization);
      TraceScope.Current.TraceEvent(TraceEventType.Information, (int) HeaderTransformer.Event.TransformRequest, "PvpToken inserted: {0}", insertedToken.OuterXml);
      XmlWriter writer = XmlWriter.Create(toStream);
      xmlDocument.WriteTo(writer);
      writer.Flush();
      if (toStream.CanSeek)
        return toStream.Position;
      return 0;
    }

    public virtual XmlElement InsertAuthorization(XmlDocument soapDocument, CustomAuthorization authorization)
    {
      XmlElement securityElement = SelectOrCreateSecurityElement(soapDocument.DocumentElement);

      XmlElement removedPvpToken = null;
      if (_tokenHandling == PvpTokenHandling.remove || _tokenHandling == PvpTokenHandling.chain ||
        (_tokenHandling == PvpTokenHandling.useConfigSetting && Properties.Settings.Default.RemoveLeftSideAuthorization) )
      {
        XmlElement existingPvpToken = null;
        while ((existingPvpToken = SelectXmlElement(securityElement, _pvpTokenTag, _pvpTokenNS)) != null)
        {
          removedPvpToken = existingPvpToken;
          securityElement.RemoveChild(existingPvpToken);
        }
      }
      
      XmlElement pvpToken = (XmlElement) soapDocument.ImportNode(authorization.SoapHeaderXmlFragment, true);
      securityElement.PrependChild(pvpToken);

      if (_tokenHandling == PvpTokenHandling.chain)
      {
        XmlNode chainedToken = GetChainedToken(removedPvpToken);
        if (chainedToken != null)
        {
          XmlElement chainedTokenToImport = (XmlElement) soapDocument.ImportNode(chainedToken, true);
          pvpToken.AppendChild(chainedTokenToImport);
        }
      }

      return pvpToken;
    }

    private XmlNode GetChainedToken(XmlElement removedPvpToken)
    {
      // neither http headers nor soap headers avaiable
      if (removedPvpToken == null && _headers==null)
        return null;

      if (_headers != null && PvpToken.DeterminePvpVersion(_headers).HasValue)
      {
        PvpToken chainedToken = new PvpToken(_headers);

        // Use http header if possible
        if (chainedToken.ParticipantId != null)
          return chainedToken.GetChainedSoapFragment();
      }

      // http headers available, but not sufficent
      if (removedPvpToken == null)
        return null;

      XmlDocument tempDoc = new XmlDocument();
      XmlNode chainedNode = tempDoc.CreateElement(_pvpChainedTokenTag, _pvpTokenNS);
      XmlNode importedToken = tempDoc.ImportNode(removedPvpToken, true);
      foreach (XmlNode child in importedToken.ChildNodes)
      {
        chainedNode.AppendChild(child);
      }
      foreach (XmlAttribute a in importedToken.Attributes)
      {
        chainedNode.Attributes.Append((XmlAttribute) a.Clone());
      }
      return chainedNode;
    }

    public virtual XmlElement SelectOrCreateSecurityElement(XmlElement soapEnvelope)
    {
      XmlElement soapHeader = SelectOrCreateHeader(soapEnvelope);

      return SelectOrCreateXmlElement(soapHeader, _securityTag, _soapAuthorization != null && _soapAuthorization.PvpVersion==PvpVersionNumber.Version18 ? _securityNSLegacy : _securityNS);
    }

    public virtual XmlElement SelectOrCreateHeader(XmlElement soapEnvelope)
    {
      return SelectOrCreateXmlElement(soapEnvelope, _headerTag, soapEnvelope.NamespaceURI);
    }

    public virtual XmlElement SelectOrCreateXmlElement(XmlElement parentElement, string elementName,
                                                       string elementNameSpace)
    {
      XmlElement xmlElement = SelectXmlElement(parentElement, elementName, elementNameSpace);
      if (xmlElement != null)
        return xmlElement;

      xmlElement = parentElement.OwnerDocument.CreateElement(elementName, elementNameSpace);
      parentElement.PrependChild(xmlElement);
      return xmlElement;
    }

    public virtual XmlElement SelectXmlElement(XmlElement parentElement, string elementName, string elementNameSpace)
    {
      if (parentElement.LocalName == elementName && parentElement.NamespaceURI == elementNameSpace)
        return parentElement;

      XmlNodeList elements = parentElement.GetElementsByTagName(elementName, elementNameSpace);

      if (elements.Count >= 1 && elements[0].NodeType == XmlNodeType.Element)
      {
        return (XmlElement) elements[0];
      }

      return null;
    }
  }
}