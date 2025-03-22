using System;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  [Serializable]
  public struct SearchGameRequest : INetworkSerializeByMemcpy
  {
    public GameRulesData Rules;
  }
}