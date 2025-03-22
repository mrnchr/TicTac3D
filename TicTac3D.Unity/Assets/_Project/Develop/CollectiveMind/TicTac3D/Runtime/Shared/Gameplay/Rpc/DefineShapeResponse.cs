using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  public struct DefinedShapeResponse : INetworkSerializeByMemcpy
  {
    public ShapeType Shape;
  }
}