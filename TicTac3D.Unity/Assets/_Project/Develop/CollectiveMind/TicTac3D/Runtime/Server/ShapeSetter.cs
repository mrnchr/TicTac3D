using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Unity.Netcode;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  public class ShapeSetter : IDisposable
  {
    private readonly INetworkBus _networkBus;
    private readonly List<CellModel> _cells;
    private readonly SessionInfo _sessionInfo;
    private readonly IRpcProvider _rpcProvider;

    public ShapeSetter(INetworkBus networkBus, List<CellModel> cells, SessionInfo sessionInfo, IRpcProvider rpcProvider)
    {
      _networkBus = networkBus;
      _cells = cells;
      _sessionInfo = sessionInfo;
      _rpcProvider = rpcProvider;

      _networkBus.SubscribeOnRpcWithParameter<SetShapeRequest>(SetShape);
    }

    private void SetShape(SetShapeRequest evt, RpcParams rpcParams)
    {
      CellModel cell = _cells.Find(c => c.Index == evt.CellIndex);
      SessionInfoAboutClient clientInfo = _sessionInfo.GetClientInfo(rpcParams.Receive.SenderClientId);
      if (clientInfo != null && clientInfo == _sessionInfo.GetMovingPlayer() && !cell.HasShape())
      {
        cell.Shape.Value = clientInfo.Shape;
        _rpcProvider.SendRequest(new UpdatedShapeResponse { CellIndex = evt.CellIndex, Shape = clientInfo.Shape });
        _sessionInfo.CurrentMove = _sessionInfo.CurrentMove == ShapeType.X ? ShapeType.O : ShapeType.X;
        _rpcProvider.SendRequest(new ChangedMoveResponse { CurrentMove = _sessionInfo.CurrentMove });
      }
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<SetShapeRequest>();
    }
  }
}