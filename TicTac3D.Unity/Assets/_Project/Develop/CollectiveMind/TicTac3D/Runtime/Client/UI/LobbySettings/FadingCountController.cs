using System;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using R3;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.LobbySettings
{
  public class FadingCountController : ITickable, IDisposable
  {
    private readonly GameRulesProvider _gameRulesProvider;
    private readonly TickableManager _ticker;

    public ReactiveProperty<bool> IsActive { get; } = new ReactiveProperty<bool>();

    public FadingCountController(GameRulesProvider gameRulesProvider,
      TickableManager ticker,
      DisposableManager disposer)
    {
      _gameRulesProvider = gameRulesProvider;
      _ticker = ticker;

      _ticker.Add(this);
      disposer.Add(this);
    }
    
    public void Tick()
    {
      IsActive.Value = _gameRulesProvider.Rules.Data.ShapeFading.IsBot();
    }

    public void Dispose()
    {
      _ticker.Remove(this);
    }
  }
}