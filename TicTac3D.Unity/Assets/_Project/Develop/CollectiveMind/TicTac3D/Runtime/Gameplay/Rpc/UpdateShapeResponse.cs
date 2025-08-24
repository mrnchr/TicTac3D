using Unity.Netcode;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public struct UpdateShapeResponse : INetworkSerializeByMemcpy
  {
    public Vector3 CellIndex;
    public ShapeType Shape;
  }
}