using System;
using CollectiveMind.TicTac3D.Runtime.Server.Session;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  public class ShapeSetter : IDisposable
  {
    private readonly INetworkBus _networkBus;
    private readonly SessionRegistry _sessionRegistry;
    private readonly IRpcProvider _rpcProvider;
    private readonly IGameRulesProcessor _gameRulesProcessor;

    public ShapeSetter(INetworkBus networkBus,
      SessionRegistry sessionRegistry,
      IRpcProvider rpcProvider,
      IGameRulesProcessor gameRulesProcessor)
    {
      _networkBus = networkBus;
      _sessionRegistry = sessionRegistry;
      _rpcProvider = rpcProvider;
      _gameRulesProcessor = gameRulesProcessor;

      _networkBus.SubscribeOnRpcWithParameter<SetShapeRequest>(SetShape);
    }

    private void SetShape(SetShapeRequest evt, RpcParams rpcParams)
    {
      GameSession session = _sessionRegistry.GetSessionByPlayerId(rpcParams.Receive.SenderClientId);
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