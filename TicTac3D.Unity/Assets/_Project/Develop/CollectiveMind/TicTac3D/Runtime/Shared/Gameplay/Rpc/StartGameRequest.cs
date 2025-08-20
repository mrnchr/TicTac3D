using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  [Serializable]
  public struct StartGameRequest : INetworkSerializeByMemcpy
  {
    public GameRulesData Rules;
  }
}