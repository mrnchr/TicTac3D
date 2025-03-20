using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell
{
  public interface ICellVisualFactory
  {
    CellVisual Create(CellModel model);
  }
}