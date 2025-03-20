using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Network
{
  public interface IRpcProvider
  {
    void SetBridge(NetworkBridge bridge);
    void SendRequest<TRequest>() where TRequest : new();
    void SendRequest<TRequest>(TRequest request) where TRequest : new();
    void SendRequest<TRequest>(RpcParams rpcParams) where TRequest : new();
    void SendRequest<TRequest>(TRequest request, RpcParams rpcParams);
  }
}