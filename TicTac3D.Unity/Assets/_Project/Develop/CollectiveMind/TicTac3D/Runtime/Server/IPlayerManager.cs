using CollectiveMind.TicTac3D.Runtime.Server.Session;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  public interface IPlayerManager
  {
    void CompleteSession(GameSession session);
  }
}