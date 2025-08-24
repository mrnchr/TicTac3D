using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public struct UpdateMoveTimeResponse : INetworkSerializeByMemcpy
  {
    public float Time;
  }
}