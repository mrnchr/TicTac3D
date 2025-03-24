using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using R3;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  [Serializable]
  public class GameInfo
  {
    public ShapeType Shape;
    public SerializableReactiveProperty<ShapeType> CurrentMove = new SerializableReactiveProperty<ShapeType>();
    public ShapeType Winner;
    public GameResultType Result;

    public bool IsMoving => Shape == CurrentMove.Value;
  }
}