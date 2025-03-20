using Unity.Multiplayer;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Network
{
  public static class NetworkRole
  {
    public static bool IsServer =
      (MultiplayerRolesManager.ActiveMultiplayerRoleMask & MultiplayerRoleFlags.Server) != 0;

    public static bool IsClient =
      (MultiplayerRolesManager.ActiveMultiplayerRoleMask & MultiplayerRoleFlags.Client) != 0;

    public static bool IsHost = IsServer && IsClient;
    public static bool IsOnlyClient = IsClient && !IsServer;
    public static bool IsOnlyServer = IsServer && !IsClient;
  }
}