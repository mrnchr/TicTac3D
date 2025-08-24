using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public struct ChangedMoveResponse : INetworkSerializeByMemcpy
  {
    public ShapeType CurrentMove;
  }
}