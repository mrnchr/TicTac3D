using System.Collections.Generic;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  public class GameplayTickableManager : IGameplayTickableManager, ITickable
  {
    private readonly List<IGameplayTickable> _tasks = new List<IGameplayTickable>();
    private readonly List<IGameplayTickable> _tickableTasks = new List<IGameplayTickable>();
    
    public bool IsPaused { get; set; }

    public GameplayTickableManager(List<IGameplayTickable> tasks)
    {
      _tasks.AddRange(tasks);
    }

    public void Add(IGameplayTickable tickable)
    {
      _tasks.Add(tickable);
    }

    public void Remove(IGameplayTickable tickable)
    {
      _tasks.Remove(tickable);
    }


    public void Tick()
    {
      if (!IsPaused)
      {
        _tickableTasks.Clear();
        _tickableTasks.AddRange(_tasks);
        _tickableTasks.ForEach(x => x.Tick());
      }
    }
  }
}