using CollectiveMind.TicTac3D.Runtime.Gameplay;

namespace CollectiveMind.TicTac3D.Runtime.Session
{
  public interface IBotBrain
  {
    CellModel GetCellToMove(GameSession session);
  }
}