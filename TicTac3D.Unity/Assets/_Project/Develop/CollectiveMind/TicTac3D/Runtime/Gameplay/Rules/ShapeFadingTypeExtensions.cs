namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public static class ShapeFadingTypeExtensions
  {
    public static bool IsPlayersOrRandom(this ShapeFadingType shapeFadingType)
    {
      return shapeFadingType.IsPlayers() || shapeFadingType == ShapeFadingType.None;
    }
    
    public static bool IsPlayers(this ShapeFadingType shapeFadingType)
    {
      return (shapeFadingType & ShapeFadingType.Players) > 0;
    }

    public static bool IsBot(this ShapeFadingType shapeFadingType)
    {
      return (shapeFadingType & ShapeFadingType.Bot) > 0;
    }
  }
}