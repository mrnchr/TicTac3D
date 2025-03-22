using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  [Serializable]
  public struct SearchGameRequest : INetworkSerializeByMemcpy
  {
    public GameRulesData Rules;
  }
}