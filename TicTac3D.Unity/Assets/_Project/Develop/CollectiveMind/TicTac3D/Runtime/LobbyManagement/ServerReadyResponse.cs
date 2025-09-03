using System;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  [Serializable]
  public struct ServerReadyResponse : INetworkSerializeByMemcpy
  {
  }
}