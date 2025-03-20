using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;

namespace CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement
{
  public enum EntityType
  {
    Cell = 1,
    Player = 2,
    XShape = Player + ShapeType.X,
    OShape = Player + ShapeType.O,
    XOShape = Player + ShapeType.XO
  }
}