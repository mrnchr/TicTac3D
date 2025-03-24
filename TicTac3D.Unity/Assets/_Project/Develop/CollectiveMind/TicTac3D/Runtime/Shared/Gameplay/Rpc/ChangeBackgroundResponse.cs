using System;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  [Serializable]
  public struct ChangeBackgroundResponse : INetworkSerializeByMemcpy
  {
    public int Index;
  }
}