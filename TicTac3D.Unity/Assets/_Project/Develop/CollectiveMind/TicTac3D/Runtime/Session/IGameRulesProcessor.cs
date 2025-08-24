using CollectiveMind.TicTac3D.Runtime.Gameplay;

namespace CollectiveMind.TicTac3D.Runtime.Session
{
  public interface IGameRulesProcessor
  {
    void SetShape(GameSession session, CellModel cell, ShapeType shape);
    void ChangeMove(GameSession session);
  }
}