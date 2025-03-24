using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using R3;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  [Serializable]
  public class GameInfo
  {
    public GameRules Rules = new GameRules();
    public ShapeType Shape;
    public SerializableReactiveProperty<int> BackgroundIndex = new SerializableReactiveProperty<int>();
    
    public SerializableReactiveProperty<ShapeType> CurrentMove = new SerializableReactiveProperty<ShapeType>();
    public SerializableReactiveProperty<float> MoveTime = new SerializableReactiveProperty<float>();
    
    public ShapeType Winner;
    public GameResultType Result;

    public bool IsMoving => Shape == CurrentMove.Value;
  }
}