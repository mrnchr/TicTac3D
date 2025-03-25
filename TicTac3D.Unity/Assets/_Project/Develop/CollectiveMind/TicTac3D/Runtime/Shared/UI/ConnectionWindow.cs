using System;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Shared.UI
{
  public class ConnectionWindow : MonoBehaviour
  {
    [SerializeField]
    private TMP_InputField _ipInputField;

    [SerializeField]
    private LocalizedString _startString;
    
    [SerializeField]
    private LocalizedString _stopString;

    [SerializeField]
    private List<ConnectionUITuple> _connectionUIElements;

    private NetworkManager _networkManager;
    private UnityTransport _transport;

    [Inject]
    public void Construct(NetworkManager networkManager)
    {
      _networkManager = networkManager;
      _transport = _networkManager.GetComponent<UnityTransport>();

      foreach (ConnectionUITuple tuple in _connectionUIElements)
      {
        tuple.Button.onClick.AddListener(tuple.Role switch
        {
          MultiplayerRoleFlags.Server => SwitchServer,
          MultiplayerRoleFlags.ClientAndServer => SwitchHost,
          MultiplayerRoleFlags.Client => SwitchClient,
          _ => throw new ArgumentOutOfRangeException(nameof(tuple.Role), tuple.Role, null)
        });
      }
    }

    private void SwitchServer()
    {
      SwitchConnection(MultiplayerRoleFlags.Server);
    }

    private void SwitchHost()
    {
      SwitchConnection(MultiplayerRoleFlags.ClientAndServer);
    }

    private void SwitchClient()
    {
      SwitchConnection(MultiplayerRoleFlags.Client);
    }

    private void SwitchConnection(MultiplayerRoleFlags role)
    {
      ChangeIp();
      
      ConnectionUITuple tuple = _connectionUIElements.Find(x => x.Role == role);
      if (_networkManager.IsListening)
      {
        _networkManager.Shutdown();
        tuple.Connected = false;
      }
      else
      {
        StartListening(role);
        tuple.Connected = true;
      }

      ChangeButtonText(role);
    }

    private void ChangeIp()
    {
      string ip = _ipInputField.text;
      string port = _transport.ConnectionData.Port.ToString();
      string[] strings = _ipInputField.text.Split(':');
      if (strings.Length == 2)
      {
        ip = strings[0];
        if (strings[1].Length == 4)
          port = strings[1];
      }

      _transport.SetConnectionData(ip, ushort.Parse(port));
    }

    private void StartListening(MultiplayerRoleFlags role)
    {
      switch (role)
      {
        case MultiplayerRoleFlags.Client:
          _networkManager.StartClient();
          break;
        case MultiplayerRoleFlags.Server:
          _networkManager.StartServer();
          break;
        case MultiplayerRoleFlags.ClientAndServer:
          _networkManager.StartHost();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(role), role, null);
      }
    }

    private void ChangeButtonText(MultiplayerRoleFlags role)
    {
      ConnectionUITuple tuple = _connectionUIElements.Find(x => x.Role == role);
      tuple.Label.StringReference.Clear();
      tuple.Label.StringReference.Add("status", tuple.Connected ? _stopString : _startString);
      tuple.Label.StringReference.Add("role", tuple.String);
    }

    private void Update()
    {
      foreach (ConnectionUITuple tuple in _connectionUIElements) 
        tuple.Button.gameObject.SetActive(tuple.Connected || !_networkManager.IsListening);
    }

    private void OnDestroy()
    {
      foreach (ConnectionUITuple tuple in _connectionUIElements)
      {
        tuple.Button.onClick.RemoveAllListeners();
      }
    }

    [Serializable]
    private class ConnectionUITuple
    {
      public MultiplayerRoleFlags Role;
      public Button Button;
      public LocalizeStringEvent Label;
      public LocalizedString String;
      public bool Connected;
    }
  }
}