namespace Egora.Stammportal.HttpReverseProxy.Mapping
{
  /// <summary>
  /// PvpTokenHandling bestimmt, wie mit vorhandenen Pvp Informationen umgegangen wird
  /// </summary>
  public enum PvpTokenHandling
  {
    /// <summary>
    /// Config Switch RemoveLeftSideAuthorization zieht
    /// </summary>
    useConfigSetting=0,
    /// <summary>
    /// Pvp Informationen werden entfernt
    /// </summary>
    remove,
    /// <summary>
    /// Pvp Informationen bleiben erhalten, im XML sind dann mehrere pvpToken enthalten
    /// </summary>
    preserve,
    /// <summary>
    /// vorhandene Pvp Informationen werden zu chained Pvp Informationen
    /// </summary>
    chain
  }
}