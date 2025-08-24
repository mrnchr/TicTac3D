using System;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  [Serializable]
  public struct StartGameRequest : INetworkSerializeByMemcpy
  {
    public GameRulesData Rules;
  }
}