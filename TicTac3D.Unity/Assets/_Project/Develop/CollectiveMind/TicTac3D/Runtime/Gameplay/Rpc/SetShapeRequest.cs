using Unity.Netcode;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public struct SetShapeRequest : INetworkSerializeByMemcpy
  {
    public Vector3 CellIndex;
  }
}