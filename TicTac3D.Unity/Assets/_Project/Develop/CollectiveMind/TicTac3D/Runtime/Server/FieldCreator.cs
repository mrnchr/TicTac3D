using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  public class FieldCreator : IDisposable
  {
    private readonly INetworkBus _networkBus;
    private readonly ICellCreator _cellCreator;

    public FieldCreator(INetworkBus networkBus, ICellCreator cellCreator)
    {
      _networkBus = networkBus;
      _cellCreator = cellCreator;

      if (NetworkRole.IsOnlyServer)
        _networkBus.SubscribeOnRpc<GameStartedEvent>(CreateField);
    }

    private void CreateField()
    {
      _cellCreator.CreateModels();
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<GameStartedEvent>();
    }
  }
}