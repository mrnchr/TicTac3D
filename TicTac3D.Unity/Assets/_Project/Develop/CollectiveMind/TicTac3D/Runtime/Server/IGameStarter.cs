using CollectiveMind.TicTac3D.Runtime.Server.Session;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  public interface IGameStarter
  {
    void StartGame(GameSession session);
  }
}