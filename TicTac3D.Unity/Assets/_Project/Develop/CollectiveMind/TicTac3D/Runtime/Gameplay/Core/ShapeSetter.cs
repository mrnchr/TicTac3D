using System;
using CollectiveMind.TicTac3D.Runtime.Network;
using CollectiveMind.TicTac3D.Runtime.Session;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public class ShapeSetter : IDisposable
  {
    private readonly INetworkBus _networkBus;
    private readonly SessionProvider _sessionProvider;
    private readonly IGameRulesProcessor _gameRulesProcessor;

    public ShapeSetter(INetworkBus networkBus,
      SessionProvider sessionProvider,
      IGameRulesProcessor gameRulesProcessor)
    {
      _networkBus = networkBus;
      _sessionProvider = sessionProvider;
      _gameRulesProcessor = gameRulesProcessor;

      _networkBus.SubscribeOnRpcWithParameters<SetShapeRequest>(SetShape);
    }

    private void SetShape(SetShapeRequest evt, RpcParams rpcParams)
    {
      GameSession session = _sessionProvider.Session;
      ShapeType playerShape = session.GetPlayerInfo(rpcParams.Receive.SenderClientId).Shape;
      CellModel cell = session.Cells.Find(c => c.Index == evt.CellIndex);
      _gameRulesProcessor.SetShape(session, cell, playerShape);
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<SetShapeRequest>();
    }
  }
}