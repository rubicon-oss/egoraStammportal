/*************************
Diese Software ist ein Beispiel (sample code) und unterliegt der Microsoft Public License. 
Die Verwendung des Codes ist unter den Bedingungen der Microsoft Public License erlaubt.
*************************
This software is sample code and is subject to the Microsoft Public License. 
You may use this code according to the conditions of the Microsoft Public License.
*************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Egora.Pvp
{
  public enum PvpAttributes
  {
    VERSION = 1,
    SECCLASS,
    PRINCIPAL_NAME,
    GIVEN_NAME,
    BIRTHDATE,
    USERID,
    GID,
    BPK,
    ENC_BPK_LIST,
    MAIL,
    TEL,
    PARTICIPANT_ID,
    PARTICIPANT_OKZ,
    OU_OKZ,
    OU_GV_OU_ID,
    OU,
    FUNCTION,
    ROLES,
    INVOICE_RECPT_ID,
    COST_CENTER_ID,
    CHARGE_CODE,
    TXID,
    ORIG_SCHEME,
    ORIG_HOST,
    ORIG_URI,
    X_AUTHENTICATE_cn,
    X_AUTHORIZE_gvOuId,
    X_AUTHORIZE_Ou,
    X_AUTHORIZE_GvOuOkz,
    Generic,
  };

  public class PvpLegacy
  {
    public static readonly Dictionary<string, PvpAttributes> HeaderMapping =
      new Dictionary<string, PvpAttributes>(StringComparer.InvariantCultureIgnoreCase)
      {
        {"X-Version", PvpAttributes.VERSION},
        {"X-AUTHENTICATE-UserId", PvpAttributes.USERID},
        {"X-AUTHENTICATE-cn", PvpAttributes.X_AUTHENTICATE_cn},
        {"X-AUTHENTICATE-mail", PvpAttributes.MAIL},
        {"X-AUTHENTICATE-participantId", PvpAttributes.PARTICIPANT_ID},
        {"X-AUTHENTICATE-gvGid", PvpAttributes.GID},
        {"X-AUTHENTICATE-gvOuId", PvpAttributes.OU_GV_OU_ID},
        {"X-AUTHENTICATE-gvOuOKZ", PvpAttributes.OU_OKZ},
        {"X-AUTHENTICATE-Ou", PvpAttributes.OU},
        {"X-AUTHENTICATE-gvFunction", PvpAttributes.FUNCTION},
        {"X-AUTHENTICATE-tel", PvpAttributes.TEL},
        {"X-AUTHENTICATE-gvSecClass", PvpAttributes.SECCLASS},
        {"X-AUTHORIZE-gvOuId", PvpAttributes.X_AUTHORIZE_gvOuId},
        {"X-AUTHORIZE-Ou", PvpAttributes.X_AUTHORIZE_Ou},
        {"X-AUTHORIZE-roles", PvpAttributes.ROLES},
        {"X-ACCOUNTING-InvoiceRecptId", PvpAttributes.INVOICE_RECPT_ID},
        {"X-ACCOUNTING-CostCenterId", PvpAttributes.COST_CENTER_ID},
        {"X-ACCOUNTING-ChargeCode", PvpAttributes.CHARGE_CODE},
        {"X-PVP-TXID", PvpAttributes.TXID},
        {"X-ORIG-SCHEME", PvpAttributes.ORIG_SCHEME},
        {"X-ORIG-HOSTINFO", PvpAttributes.ORIG_HOST},
        {"X-ORIG-URI", PvpAttributes.ORIG_URI},
      };

    public static List<PvpAttributes> AttributeOrder19 = new List<PvpAttributes>()
      {
        PvpAttributes.VERSION,
        PvpAttributes.PARTICIPANT_ID,
        PvpAttributes.USERID,
        PvpAttributes.X_AUTHENTICATE_cn,
        PvpAttributes.OU_GV_OU_ID, 
        PvpAttributes.OU, 
        PvpAttributes.OU_OKZ, 
        PvpAttributes.SECCLASS, 
        PvpAttributes.MAIL, 
        PvpAttributes.TEL, 
        PvpAttributes.GID, 
        PvpAttributes.FUNCTION, 
        PvpAttributes.BPK, 
        PvpAttributes.X_AUTHORIZE_gvOuId, 
        PvpAttributes.X_AUTHORIZE_Ou, 
        PvpAttributes.X_AUTHORIZE_GvOuOkz, 
        PvpAttributes.ROLES, 
        PvpAttributes.INVOICE_RECPT_ID,
        PvpAttributes.COST_CENTER_ID,
        PvpAttributes.CHARGE_CODE,
        PvpAttributes.TXID,
        PvpAttributes.ORIG_SCHEME,
        PvpAttributes.ORIG_HOST,
        PvpAttributes.ORIG_URI,
        PvpAttributes.Generic,
      };

  };

}

