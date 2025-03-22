using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;

namespace CollectiveMind.TicTac3D.Runtime.Server.Session
{
  public interface IBotBrain
  {
    CellModel GetCellToMove(GameSession session);
  }
}