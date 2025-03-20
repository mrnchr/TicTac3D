using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;

namespace CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement
{
  public enum EntityType
  {
    Cell = 1,
    NetworkBridge = 2,
    XShape = NetworkBridge + ShapeType.X,
    OShape = NetworkBridge + ShapeType.O,
    XOShape = NetworkBridge + ShapeType.XO
  }
}