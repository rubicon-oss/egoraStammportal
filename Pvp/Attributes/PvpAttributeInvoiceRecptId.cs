using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egora.Pvp.Attributes
{
  public class PvpAttributeInvoiceRecptId : PvpAttribute
  {
    public PvpAttributeInvoiceRecptId()
      : base(

        friendlyName: "INVOICE-RECPT-ID"
        , index: PvpAttributes.INVOICE_RECPT_ID
        , samlAttributeName: "urn:oid:1.2.40.0.10.2.1.1.261.40"
        , availableInVersions: new PvpVersion[] { PvpVersion.Version18, PvpVersion.Version19, PvpVersion.Version20, PvpVersion.Version21 }
        , headerNames: new Dictionary<PvpVersion, string>()
                      {
                         {PvpVersion.Version18, "X-ACCOUNTING-InvoiceRecptId"}
                        ,{PvpVersion.Version19, "X-ACCOUNTING-InvoiceRecptId"}
                        ,{PvpVersion.Version20, "X-PVP-INVOICE-RECPT-ID"}
                        ,{PvpVersion.Version21, "X-PVP-INVOICE-RECPT-ID"}
                      }
      , soapElementName: "ChargeCode"
        )
    {}

    public PvpAttributeInvoiceRecptId(string value)
      : this()
    {
      Value = value;
    }

    public override void CheckValue(string value)
    {
      StringMaxLenCheck(value, 64);
    }
  }
}
