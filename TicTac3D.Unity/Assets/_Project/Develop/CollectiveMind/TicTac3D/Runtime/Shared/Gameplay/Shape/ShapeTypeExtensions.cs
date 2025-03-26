namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape
{
  public static class ShapeTypeExtensions
  {
    public static bool IsPlayer(this ShapeType shape)
    {
      return shape is >= ShapeType.X and <= ShapeType.O;
    }
  }
}