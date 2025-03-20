using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  public struct ChangedMoveResponse : INetworkSerializeByMemcpy
  {
    public ShapeType CurrentMove;
  }
}