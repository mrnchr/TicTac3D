using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Network;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Session
{
  public class MoveTimeUpdater : ITickable
  {
    private readonly SessionProvider _sessionProvider;
    private readonly IRpcProvider _rpcProvider;
    private readonly IGameRulesProcessor _gameRulesProcessor;

    public MoveTimeUpdater(SessionProvider sessionProvider, IRpcProvider rpcProvider, IGameRulesProcessor gameRulesProcessor)
    {
      _sessionProvider = sessionProvider;
      _rpcProvider = rpcProvider;
      _gameRulesProcessor = gameRulesProcessor;
    }

    public void Tick()
    {
      GameSession session = _sessionProvider.Session;
      if (session != null && session.Status == SessionState.Playing && session.Rules.Data.MoveTime > 0
        && session.CurrentMove != ShapeType.XO && session.MoveTime > 0)
      {
        session.MoveTime = Mathf.Max(session.MoveTime - Time.deltaTime, 0);
        _rpcProvider.SendRequest(new UpdateMoveTimeResponse { Time = session.MoveTime }, session.Target);

        if (session.MoveTime == 0)
          _gameRulesProcessor.ChangeMove(session);
      }
    }
  }
}