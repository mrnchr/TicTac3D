using System;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Server.Session;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  public class PlayerManager : IDisposable
  {
    private readonly NetworkManager _networkManager;
    private readonly SessionRegistry _sessionRegistry;
    private readonly IGameStarter _gameStarter;

    public PlayerManager(NetworkManager networkManager, SessionRegistry sessionRegistry, IGameStarter gameStarter)
    {
      _networkManager = networkManager;
      _sessionRegistry = sessionRegistry;
      _gameStarter = gameStarter;


      if (NetworkRole.IsServer)
        _networkManager.OnConnectionEvent += OnClientConnected;
    }

    private void OnClientConnected(NetworkManager network, ConnectionEventData connectionEvent)
    {
      if (connectionEvent.EventType != ConnectionEvent.ClientConnected)
        return;
      
      GameSession session = _sessionRegistry.GetWaitingSession();
      if (session != null)
      {
        session.AddPlayer(connectionEvent.ClientId);
        session.Target =
          _networkManager.RpcTarget.Group(session.Players.Select(x => x.PlayerId), RpcTargetUse.Persistent);
        _gameStarter.StartGame(session);
        session.Status = SessionState.Playing;
        return;
      }

      session = new GameSession();
      _sessionRegistry.Sessions.Add(session);
      session.AddPlayer(connectionEvent.ClientId);
      session.Status = SessionState.Waiting;
    }

    public void Dispose()
    {
      _networkManager.OnConnectionEvent -= OnClientConnected;
    }
  }
}