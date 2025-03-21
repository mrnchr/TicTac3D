using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  [Serializable]
  public struct FinishGameResponse : INetworkSerializeByMemcpy
  {
    public ShapeType Winner;
  }
}