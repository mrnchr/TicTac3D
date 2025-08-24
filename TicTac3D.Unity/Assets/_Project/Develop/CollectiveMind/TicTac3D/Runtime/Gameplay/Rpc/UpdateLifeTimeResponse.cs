using System;
using Unity.Netcode;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  [Serializable]
  public struct UpdateLifeTimeResponse : INetworkSerializeByMemcpy
  {
    public Vector3 CellIndex;
    public int LifeTime;
  }
}