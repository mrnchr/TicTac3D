using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Server.Session
{
  public class MoveTimeUpdater : ITickable
  {
    private readonly SessionRegistry _sessionRegistry;
    private readonly IRpcProvider _rpcProvider;
    private readonly IGameRulesProcessor _gameRulesProcessor;

    public MoveTimeUpdater(SessionRegistry sessionRegistry, IRpcProvider rpcProvider, IGameRulesProcessor gameRulesProcessor)
    {
      _sessionRegistry = sessionRegistry;
      _rpcProvider = rpcProvider;
      _gameRulesProcessor = gameRulesProcessor;
    }

    public void Tick()
    {
      foreach (GameSession session in _sessionRegistry.Sessions.FindAll(x => x.Status == SessionState.Playing
        && x.Rules.Data.MoveTime > 0 && x.CurrentMove != ShapeType.XO && x.MoveTime > 0))
      {
        session.MoveTime = Mathf.Max(session.MoveTime - Time.deltaTime, 0);
        _rpcProvider.SendRequest(new UpdateMoveTimeResponse { Time = session.MoveTime }, session.Target);

        if (session.MoveTime == 0)
        {
          _gameRulesProcessor.ChangeMove(session);
        }
      }
    }
  }
}