using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Network
{
  public interface IRpcProvider
  {
    bool IsReady { get; }
    void SetBridge(NetworkBridge bridge);
    void SendRequest<TRequest>() where TRequest : struct;
    void SendRequest<TRequest>(TRequest request) where TRequest : struct;
    void SendRequest<TRequest>(RpcParams rpcParams) where TRequest : struct;
    void SendRequest<TRequest>(TRequest request, RpcParams rpcParams) where TRequest : struct;
    void ChangeVariable<TVariable>(TVariable value) where TVariable : struct;
  }
}