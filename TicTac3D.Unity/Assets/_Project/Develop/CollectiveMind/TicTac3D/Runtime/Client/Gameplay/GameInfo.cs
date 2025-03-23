using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  public class GameInfo
  {
    public ShapeType Shape;
    public ShapeType CurrentMove;
    public ShapeType Winner;
    public GameResultType Result;

    public bool IsMoving => Shape == CurrentMove;
  }
}