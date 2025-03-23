using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI
{
  public class GameResultWindow : BaseWindow
  {
    [SerializeField]
    private LocalizeStringEvent _resultTitle;

    [SerializeField]
    private LocalizeStringEvent _resultDescription;

    [SerializeField]
    private Button _menuButton;
    
    [SerializeField]
    private string _exclamationVariableName;
      
    [SerializeField]
    private string _manyShapesVariableName;

    [SerializeField]
    private LocalizedString _xShapeLocalizedString;

    [SerializeField]
    private LocalizedString _oShapeLocalizedString;

    [SerializeField]
    private NestedVariablesGroup _shapesVariablesGroup;

    [SerializeField]
    private List<LocalizedGameResult> _resultStrings;

    private IGameStateMachine _gameStateMachine;
    private GameInfo _gameInfo;

    [Inject]
    public void Construct(IGameStateMachine gameStateMachine, GameInfo gameInfo)
    {
      _gameStateMachine = gameStateMachine;
      _gameInfo = gameInfo;

      _menuButton.AddListener(ExitToMenu);
    }

    protected override UniTask OnOpened()
    {
      LocalizedGameResult localization = _resultStrings.Find(x => x.Result == _gameInfo.Result);
      _resultTitle.StringReference = localization.ResultTitle;
      if (_gameInfo.Result != GameResultType.Draw)
      {
        localization.ResultDescription.Clear();
        localization.ResultDescription.Add(_exclamationVariableName,
          new StringVariable { Value = _gameInfo.Result == GameResultType.Win ? "!" : "" });
        localization.ResultDescription.Add(_manyShapesVariableName,
          _gameInfo.Winner == ShapeType.O ? _oShapeLocalizedString : _xShapeLocalizedString);
        localization.ResultDescription.RefreshString();
      }
      
      _resultDescription.StringReference = localization.ResultDescription;
      
      return base.OnOpened();
    }

    private void ExitToMenu()
    {
      _gameStateMachine.SwitchState<MenuGameState>().Forget();
    }

    [Serializable]
    private class LocalizedGameResult
    {
      public GameResultType Result;
      public LocalizedString ResultTitle;
      public LocalizedString ResultDescription;
    }
  }
}