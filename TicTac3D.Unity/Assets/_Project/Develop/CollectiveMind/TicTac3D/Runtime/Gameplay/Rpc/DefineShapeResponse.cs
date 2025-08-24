using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public struct DefinedShapeResponse : INetworkSerializeByMemcpy
  {
    public ShapeType Shape;
  }
}