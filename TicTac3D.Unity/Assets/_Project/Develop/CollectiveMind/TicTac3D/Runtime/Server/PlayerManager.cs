using System;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Server.Session;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  public class PlayerManager : IPlayerManager, IDisposable
  {
    private readonly NetworkManager _networkManager;
    private readonly SessionRegistry _sessionRegistry;
    private readonly IGameStarter _gameStarter;
    private readonly INetworkBus _networkBus;
    private readonly IRpcProvider _rpcProvider;

    public PlayerManager(NetworkManager networkManager,
      SessionRegistry sessionRegistry,
      IGameStarter gameStarter,
      INetworkBus networkBus,
      IRpcProvider rpcProvider)
    {
      _networkManager = networkManager;
      _sessionRegistry = sessionRegistry;
      _gameStarter = gameStarter;
      _networkBus = networkBus;
      _rpcProvider = rpcProvider;

      if (NetworkRole.IsServer)
      {
        _networkManager.OnConnectionEvent += OnClientDisconnected;
        _networkBus.SubscribeOnRpcWithParameters<SearchGameRequest>(AddToOrCreateSession);
        _networkBus.SubscribeOnRpcWithParameter<StopSearchGameRequest>(LeavePlayerFromSession);
        _networkBus.SubscribeOnRpcWithParameter<LeaveGameRequest>(LeavePlayerFromSession);
      }
    }

    private void OnClientDisconnected(NetworkManager network, ConnectionEventData connectionEvent)
    {
      if (connectionEvent.EventType == ConnectionEvent.ClientDisconnected)
        CompleteOrRemoveSession(connectionEvent.ClientId);
    }

    private void AddToOrCreateSession(SearchGameRequest request, RpcParams rpcParams)
    {
      GameSession session = _sessionRegistry.Sessions.FirstOrDefault(x =>
        x.Status == SessionState.Waiting && x.Players[0].GameRules.Match(request.Rules));
      if (session != null)
      {
        session.AddPlayer(rpcParams.Receive.SenderClientId, request.Rules);
        session.Target =
          _networkManager.RpcTarget.Group(session.Players.Select(x => x.PlayerId), RpcTargetUse.Persistent);
        _gameStarter.StartGame(session);
        session.Status = SessionState.Playing;
        return;
      }

      session = new GameSession();
      _sessionRegistry.Sessions.Add(session);
      session.AddPlayer(rpcParams.Receive.SenderClientId, request.Rules);
      session.Status = SessionState.Waiting;
    }

    private void LeavePlayerFromSession(RpcParams rpcParams)
    {
      CompleteOrRemoveSession(rpcParams.Receive.SenderClientId);
    }

    private void CompleteOrRemoveSession(ulong clientId)
    {
      GameSession session = _sessionRegistry.GetSessionByPlayerId(clientId);
      if (session == null)
        return;

      if (session.Status == SessionState.Waiting)
      {
        _sessionRegistry.Sessions.Remove(session);
      }
      else if (session.Status == SessionState.Playing)
      {
        session.Players.RemoveAll(x => x.PlayerId == clientId);
        session.Status = SessionState.Completed;
        _rpcProvider.SendRequest(new FinishGameResponse { Winner = session.Players[0].Shape },
          _networkManager.RpcTarget.Single(session.Players[0].PlayerId, RpcTargetUse.Persistent));
      }
      else if (session.Status == SessionState.Completed)
      {
        session.Players.RemoveAll(x => x.PlayerId == clientId);
        if (session.Players.Count == 0)
          _sessionRegistry.Sessions.Remove(session);
      }
    }

    public void Dispose()
    {
      _networkManager.OnConnectionEvent -= OnClientDisconnected;
      _networkBus.UnsubscribeFromRpc<SearchGameRequest>();
      _networkBus.UnsubscribeFromRpc<StopSearchGameRequest>();
      _networkBus.UnsubscribeFromRpc<LeaveGameRequest>();
    }
  }
}