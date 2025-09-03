using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class AuthorizationService
  {
    public async UniTask<AsyncResult> SignIn(CancellationToken token = default(CancellationToken))
    {
      while (!AuthenticationService.Instance.IsSignedIn)
      {
        if (token.IsCancellationRequested)
          return AsyncReturn.Cancel();

        try
        {
          await AuthenticationService.Instance.SignInAnonymouslyAsync();
          if (token.IsCancellationRequested)
          {
            AuthenticationService.Instance.SignOut();
            return AsyncReturn.Cancel();
          }
          
          Debug.Log("Signed in.");
        }
        catch (RequestFailedException)
        {
          Debug.Log("Can not sign in. Retrying...");
          await UniTask.WaitForSeconds(0.5f, cancellationToken: token).SuppressCancellationThrow();
        }
      }

      return AsyncReturn.Ok();
    }
  }
}