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

    public ShapeSetter(INetworkBus networkBus, SessionRegistry sessionRegistry, IRpcProvider rpcProvider)
    {
      _networkBus = networkBus;
      _sessionRegistry = sessionRegistry;
      _rpcProvider = rpcProvider;

      _networkBus.SubscribeOnRpcWithParameter<SetShapeRequest>(SetShape);
    }

    private void SetShape(SetShapeRequest evt, RpcParams rpcParams)
    {
      GameSession session = _sessionRegistry.GetSessionByPlayerId(rpcParams.Receive.SenderClientId);
      CellModel cell = session.Cells.Find(c => c.Index == evt.CellIndex);
      PlayerInfo clientInfo = session.GetPlayerInfo(rpcParams.Receive.SenderClientId);
      if (clientInfo != null && clientInfo == session.GetMovingPlayer() && !cell.HasShape())
      {
        cell.Shape.Value = clientInfo.Shape;
        _rpcProvider.SendRequest(new UpdatedShapeResponse { CellIndex = evt.CellIndex, Shape = clientInfo.Shape },
          session.Target);
        session.CurrentMove = session.CurrentMove == ShapeType.X ? ShapeType.O : ShapeType.X;
        _rpcProvider.SendRequest(new ChangedMoveResponse { CurrentMove = session.CurrentMove }, session.Target);
      }
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<SetShapeRequest>();
    }
  }
}