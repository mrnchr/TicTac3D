using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.WindowManagement;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public class HudWindow : BaseWindow
  {
    [SerializeField]
    private Button _settingsButton;

    [SerializeField]
    private LocalizeStringEvent _currentMoveLabel;

    [SerializeField]
    private LocalizedString _yourMoveString;

    [SerializeField]
    private List<ShapeLocalizationTuple> _localizedShapeMoves;

    [SerializeField]
    private Image _timerImage;

    [SerializeField]
    private TMP_Text _timerLabel;

    private IWindowManager _windowManager;
    private IGameplayTickableManager _gameplayTickableManager;
    private GameInfo _gameInfo;
    private IConfigLoader _configLoader;
    private ShapeConfig _config;
    private ConfirmationPopup _confirmationPopup;
    private TMP_Text _currentMoveText;

    [Inject]
    public void Construct(IWindowManager windowManager,
      IGameplayTickableManager gameplayTickableManager,
      GameInfo gameInfo,
      IConfigLoader configLoader)
    {
      _windowManager = windowManager;
      _gameplayTickableManager = gameplayTickableManager;
      _gameInfo = gameInfo;
      _configLoader = configLoader;
      _config = configLoader.LoadConfig<ShapeConfig>();
      _confirmationPopup = GetComponentInChildren<ConfirmationPopup>(true);
      _currentMoveText = _currentMoveLabel.GetComponent<TMP_Text>();

      _settingsButton.AddListener(OpenPauseWindow);
      _gameInfo.CurrentMove.Subscribe(ChangeCurrentMoveText);
      _gameInfo.MoveTime.Subscribe(UpdateTime);
    }

    protected override UniTask OnInvisible()
    {
      if (_confirmationPopup.gameObject.activeSelf)
        _confirmationPopup.Deny(false);

      return base.OnInvisible();
    }

    private void OpenPauseWindow()
    {
      _gameplayTickableManager.IsPaused = true;
      _windowManager.OpenWindow<SettingsWindow>();
    }

    private void ChangeCurrentMoveText(ShapeType shape)
    {
      _currentMoveLabel.StringReference =
        _gameInfo.IsMoving ? _yourMoveString : _localizedShapeMoves.Find(x => x.Shape == shape).String;
      _currentMoveText.color = _config.GetDataForShape(shape).Color;

      SetTimer(shape);
    }

    private void SetTimer(ShapeType shape)
    {
      bool isActive = _gameInfo.Rules.Data.MoveTime > 0 && shape != ShapeType.XO;
      _timerImage.gameObject.SetActive(isActive);
      _timerLabel.gameObject.SetActive(isActive);

      if (shape != ShapeType.XO)
      {
        ShapeTuple shapeData = _config.GetDataForShape(shape);
        _timerImage.sprite = shapeData.TimerSprite;
        _timerLabel.color = shapeData.Color;
      }
    }

    private void UpdateTime(float time)
    {
      _timerLabel.text = $"{time / 60:0}:{time % 60:00}";
    }

    private void OnDestroy()
    {
      _settingsButton.RemoveListener(OpenPauseWindow);
      _configLoader.UnloadConfig<ShapeConfig>();
    }
  }

  [Serializable]
  public struct ShapeLocalizationTuple
  {
    public ShapeType Shape;
    public LocalizedString String;
  }
}