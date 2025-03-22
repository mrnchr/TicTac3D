using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules
{
  [Serializable]
  public struct GameRulesData : INetworkSerializeByMemcpy
  {
    public ShapeType DesiredShape; 
    public int BotMoveCount;
    public float MoveTime;
    public ShapeFadingType ShapeFading;
    public int FadingMoveCount;
  }
}