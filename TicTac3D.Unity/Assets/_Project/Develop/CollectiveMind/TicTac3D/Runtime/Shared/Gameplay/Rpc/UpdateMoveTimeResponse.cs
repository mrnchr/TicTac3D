using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  public struct UpdateMoveTimeResponse : INetworkSerializeByMemcpy
  {
    public float Time;
  }
}