namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public interface ICellVisualFactory
  {
    CellVisual Create(CellModel model);
  }
}