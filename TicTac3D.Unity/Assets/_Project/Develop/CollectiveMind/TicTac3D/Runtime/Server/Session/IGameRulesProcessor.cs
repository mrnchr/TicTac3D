using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;

namespace CollectiveMind.TicTac3D.Runtime.Server.Session
{
  public interface IGameRulesProcessor
  {
    void SetShape(GameSession session, CellModel cell, ShapeType shape);
  }
}