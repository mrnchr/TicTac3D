using System;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  [Serializable]
  public struct StartGameResponse : INetworkSerializeByMemcpy
  {
    public GameRulesData GameRules;
    public int BackgroundIndex;
  }
}