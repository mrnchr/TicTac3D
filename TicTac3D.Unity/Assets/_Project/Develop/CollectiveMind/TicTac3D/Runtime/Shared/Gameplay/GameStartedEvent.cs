using System;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  [Serializable]
  public struct GameStartedEvent : INetworkSerializeByMemcpy
  {
  }
}