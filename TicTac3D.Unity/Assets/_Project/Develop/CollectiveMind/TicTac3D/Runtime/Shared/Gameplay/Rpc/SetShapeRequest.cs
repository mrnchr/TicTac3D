using Unity.Netcode;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  public struct SetShapeRequest : INetworkSerializeByMemcpy
  {
    public Vector3 CellIndex;
  }
}