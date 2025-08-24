namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public interface IGameplayTickableManager
  {
    bool IsPaused { get; set; }
    void Add(IGameplayTickable tickable);
    void Remove(IGameplayTickable tickable);
  }
}