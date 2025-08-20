using System;
using System.Collections.Generic;
using System.Threading;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.LobbyManagement
{
  public class LobbyManager
  {
    private readonly NetworkManager _networkManager;
    private readonly IRpcProvider _rpcProvider;

    public LobbyManager(NetworkManager networkManager, IRpcProvider rpcProvider)
    {
      _networkManager = networkManager;
      _rpcProvider = rpcProvider;
    }

    public async UniTask Initialize()
    {
      await UnityServices.InitializeAsync();
    }

    public async UniTask InitializeLobby(GameRulesData userRules, CancellationToken token = default(CancellationToken))
    {
      if (!AuthenticationService.Instance.IsSignedIn)
      {
        await Authorize(token);
      }

      QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync();
      if (token.IsCancellationRequested)
        return;

      Lobby matchedLobby = null;
      foreach (Lobby lobby in lobbies.Results)
      {
        var rules = JsonConvert.DeserializeObject<GameRulesData>(lobby.Data["Rules"].Value);
        if (GameRulesData.Match(userRules, rules))
          matchedLobby = lobby;
      }

      if (matchedLobby != null)
      {
        Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(matchedLobby.Id);
        string joinCode = lobby.Data["JoinCode"].Value;
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        _networkManager.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("wss"));
        _networkManager.GetComponent<UnityTransport>().UseWebSockets = true;
        _networkManager.StartClient();
        await UniTask.WaitUntil(() => _rpcProvider.IsReady, cancellationToken: token);
        _rpcProvider.SendRequest(new StartGameRequest { Rules = userRules });
      }
      else
      {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        await LobbyService.Instance.CreateLobbyAsync("LobbyName", 2, new CreateLobbyOptions
        {
          Data = new Dictionary<string, DataObject>
          {
            {
              "Rules",
              new DataObject(DataObject.VisibilityOptions.Public, JsonConvert.SerializeObject(userRules))
            },
            {
              "JoinCode",
              new DataObject(DataObject.VisibilityOptions.Member, joinCode)
            }
          }
        });

        // await LobbyService.Instance.SubscribeToLobbyEventsAsync("LobbyName", _callbacks);
        _networkManager.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("wss"));
        _networkManager.GetComponent<UnityTransport>().UseWebSockets = true;
        _networkManager.StartHost();
        await UniTask.WaitUntil(() => _rpcProvider.IsReady, cancellationToken: token);
        _rpcProvider.SendRequest(new StartGameRequest { Rules = userRules }, _networkManager.RpcTarget.Server);
      }

      // TODO: выходить из лобби когда сервер отвалился
    }

    public async UniTask Authorize(CancellationToken token = default(CancellationToken))
    {
      while (!AuthenticationService.Instance.IsAuthorized)
      {
        if (token.IsCancellationRequested)
          break;

        try
        {
          await AuthenticationService.Instance.SignInAnonymouslyAsync();
          Debug.Log("Signed in.");
        }
        catch (RequestFailedException e)
        {
          Debug.Log("Can not sign in. Retrying...");
          await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
        }
      }
    }
  }
}