namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  public interface IGameplayTickableManager
  {
    bool IsPaused { get; set; }
    void Add(IGameplayTickable tickable);
    void Remove(IGameplayTickable tickable);
  }
}