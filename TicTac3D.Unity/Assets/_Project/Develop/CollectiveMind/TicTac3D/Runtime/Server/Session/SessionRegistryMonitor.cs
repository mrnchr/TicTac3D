using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Server.Session
{
  public class SessionRegistryMonitor : MonoBehaviour
  {
    [SerializeField]
    private SessionRegistry _sessionRegistry;

    [Inject]
    public void Construct(SessionRegistry sessionRegistry)
    {
      _sessionRegistry = sessionRegistry;
    }
  }
}