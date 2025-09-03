using System;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Network;
using CollectiveMind.TicTac3D.Runtime.Session;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public class PlayerManager : IPlayerManager, IDisposable
  {
    private readonly NetworkManager _networkManager;
    private readonly SessionProvider _sessionProvider;
    private readonly IGameStarter _gameStarter;
    private readonly INetworkBus _networkBus;
    private readonly IRpcProvider _rpcProvider;

    public PlayerManager(NetworkManager networkManager,
      SessionProvider sessionProvider,
      IGameStarter gameStarter,
      INetworkBus networkBus,
      IRpcProvider rpcProvider)
    {
      _networkManager = networkManager;
      _sessionProvider = sessionProvider;
      _gameStarter = gameStarter;
      _networkBus = networkBus;
      _rpcProvider = rpcProvider;

      _networkManager.OnConnectionEvent += OnClientDisconnected;
      _networkBus.SubscribeOnRpcWithParameters<StartGameRequest>(AddOrCreateSession);
      _networkBus.SubscribeOnRpcWithParameter<LeaveGameRequest>(LeavePlayerFromSession);
    }

    private void OnClientDisconnected(NetworkManager network, ConnectionEventData connectionEvent)
    {
      if (network.ShutdownInProgress)
        _sessionProvider.Session = null;

      if (connectionEvent.EventType == ConnectionEvent.ClientDisconnected)
        CompleteOrRemoveSession(connectionEvent.ClientId);
    }

    private void AddOrCreateSession(StartGameRequest request, RpcParams rpcParams)
    {
      GameSession session = _sessionProvider.Session;
      if (session == null)
      {
        session = new GameSession();
        _sessionProvider.Session = session;
        session.AddPlayer(rpcParams.Receive.SenderClientId, request.Rules);
        session.Status = SessionState.Waiting;
        return;
      }

      session.AddPlayer(rpcParams.Receive.SenderClientId, request.Rules);
      session.Target =
        _networkManager.RpcTarget.Group(session.Players.Select(x => x.PlayerId), RpcTargetUse.Persistent);
      _gameStarter.StartGame(session);
      session.Status = SessionState.Playing;
    }

    private void LeavePlayerFromSession(RpcParams rpcParams)
    {
      CompleteOrRemoveSession(rpcParams.Receive.SenderClientId);
    }

    private void CompleteOrRemoveSession(ulong clientId)
    {
      GameSession session = _sessionProvider.Session;
      if (session == null)
        return;

      if (session.Status == SessionState.Waiting)
      {
        _sessionProvider.Session = null;
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
          _sessionProvider.Session = null;
      }
    }

    public void Dispose()
    {
      _networkManager.OnConnectionEvent -= OnClientDisconnected;
      _networkBus.UnsubscribeFromRpc<StartGameRequest>();
      _networkBus.UnsubscribeFromRpc<LeaveGameRequest>();
    }
  }
}