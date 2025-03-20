using System;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Server.Session;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Unity.Netcode;
using UnityEngine;

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
      if (connectionEvent.EventType == ConnectionEvent.ClientConnected)
        AddOrCreateSession(connectionEvent);
      else if (connectionEvent.EventType == ConnectionEvent.ClientDisconnected)
        CompleteOrRemoveSession(connectionEvent);
    }

    private void AddOrCreateSession(ConnectionEventData connectionEvent)
    {
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

    private void CompleteOrRemoveSession(ConnectionEventData connectionEvent)
    {
      GameSession session = _sessionRegistry.GetSessionByPlayerId(connectionEvent.ClientId);
      if (session.Status == SessionState.Waiting)
      {
        _sessionRegistry.Sessions.Remove(session);
      }
      else if (session.Status == SessionState.Playing)
      {
        session.Players.RemoveAll(x => x.PlayerId == connectionEvent.ClientId);
        session.Status = SessionState.Completed;
        _networkManager.DisconnectClient(session.Players[0].PlayerId);
      }
      else if (session.Status == SessionState.Completed)
      {
        session.Players.RemoveAll(x => x.PlayerId == connectionEvent.ClientId);
        if(session.Players.Count == 0)
          _sessionRegistry.Sessions.Remove(session);
      }
    }

    public void Dispose()
    {
      _networkManager.OnConnectionEvent -= OnClientConnected;
    }
  }
}