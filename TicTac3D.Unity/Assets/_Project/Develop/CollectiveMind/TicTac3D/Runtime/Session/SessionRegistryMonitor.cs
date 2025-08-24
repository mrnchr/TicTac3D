using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Session
{
  public class SessionRegistryMonitor : MonoBehaviour
  {
    [SerializeField]
    private SessionProvider _sessionProvider;

    [Inject]
    public void Construct(SessionProvider sessionProvider)
    {
      _sessionProvider = sessionProvider;
    }
  }
}