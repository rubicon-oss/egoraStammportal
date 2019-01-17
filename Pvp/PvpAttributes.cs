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

  /// <summary>
  /// Enum for Pvp Attributes for general access (saml, http, soap)
  /// </summary>
  public enum PvpAttributes
  {
    /// <summary>
    /// Http Header 1.x 'X-Version'
    /// </summary>
    VERSION = 1,
    /// <summary>
    /// Http Header 1.x 'X-AUTHENTICATE-gvSecClass'
    /// </summary>
    SECCLASS,
    PRINCIPAL_NAME,
    GIVEN_NAME,
    BIRTHDATE,
    /// <summary>
    /// Http Heaader 1.x 'X-AUTHENTICATE-USERID'
    /// </summary>
    USERID,
    /// <summary>
    /// Http Header 1.x 'X-AUTHENTICATE-GVGID'
    /// </summary>
    GID,
    /// <summary>
    /// Http Header 1.x 'X-AUTHENTICATE-GVBPK'
    /// </summary>
    BPK,
    ENC_BPK_LIST,
    /// <summary>
    /// Http Header 1.x 'X-AUTHENTICATE-MAIL'
    /// </summary>
    MAIL,
    /// <summary>
    /// Http Header 1.x 'X-AUTHENTICATE-TEL'
    /// </summary>
    TEL,
    /// <summary>
    /// Http Header 1.x 'X-AUTHENTICATE-PARTICIPANTID'
    /// </summary>
    PARTICIPANT_ID,
    PARTICIPANT_OKZ,
    /// <summary>
    /// Http Header 1.9.2 'X-AUTHENTICATE-GVOUOKZ'
    /// </summary>
    OU_OKZ,
    /// <summary>
    /// Http Header 1.x 'X-AUTHENTICATE-GVOUID'
    /// </summary>
    OU_GV_OU_ID,
    /// <summary>
    /// Http Header 1.x 'X-AUTHENTICATE-OU'
    /// </summary>
    OU,
    /// <summary>
    /// Http Header 1.x 'X-AUTHENTICATE-GVFUNCTION'
    /// </summary>
    FUNCTION,
    /// <summary>
    /// Http Header 1.x 'X-AUTHORIZE-ROLES'
    /// </summary>
    ROLES,
    /// <summary>
    /// Http Header 1.x 'X-ACCOUNTING-INVOICERECPTID'
    /// </summary>
    INVOICE_RECPT_ID,
    /// <summary>
    /// Http Header 1.x 'X-ACCOUNTING-COSTCENTERID'
    /// </summary>
    COST_CENTER_ID,
    /// <summary>
    /// Http Header 1.x 'X-ACCOUNTING-CHARGECODE'
    /// </summary>
    CHARGE_CODE,
    /// <summary>
    /// Http Header 1.9 'X-TXID'
    /// </summary>
    TXID,
    /// <summary>
    /// Http Header 1.9 'X-ORIG-SCHEME'
    /// </summary>
    ORIG_SCHEME,
    /// <summary>
    /// Http Header 1.9 'X-ORIG-HOSTINFO'
    /// </summary>
    ORIG_HOST,
    /// <summary>
    /// Http Header 1.9 'X-ORIG-URI'
    /// </summary>
    ORIG_URI,
    /// <summary>
    /// Http Header 1.x 'X-AUTHENTICATE-cn'
    /// </summary>
    X_AUTHENTICATE_cn,
    /// <summary>
    /// Http Header 1.x 'X-AUTHORIZE-gvOuId'
    /// </summary>
    X_AUTHORIZE_gvOuId,
    /// <summary>
    /// Http Header 1.x 'X-AUTHORIZE-Ou'
    /// </summary>
    X_AUTHORIZE_Ou,
    /// <summary>
    /// Http Header 1.9 'X-AUTHORIZE-gvOuOKZ'
    /// </summary>
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

