using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Client.UI;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  public class FieldCreator : IDisposable
  {
    private readonly INetworkBus _networkBus;
    private readonly ICellCreator _cellCreator;
    private readonly ICellVisualFactory _cellVisualFactory;
    private readonly List<CellModel> _cells;
    private readonly GameInfo _gameInfo;
    private readonly List<CellVisual> _cellVisuals;
    private readonly IWindowManager _windowManager;
    private readonly MainMenu _mainMenu;

    public FieldCreator(INetworkBus networkBus,
      ICellCreator cellCreator,
      ICellVisualFactory cellVisualFactory,
      List<CellModel> cells,
      GameInfo gameInfo,
      List<CellVisual> cellVisuals,
      IWindowManager windowManager)
    {
      _networkBus = networkBus;
      _cellCreator = cellCreator;
      _cellVisualFactory = cellVisualFactory;
      _cells = cells;
      _gameInfo = gameInfo;
      _cellVisuals = cellVisuals;
      _windowManager = windowManager;

      _mainMenu = Object.FindAnyObjectByType<MainMenu>(FindObjectsInactive.Include);

      _networkBus.SubscribeOnRpcWithParameter<StartedGameResponse>(CreateField);
      _networkBus.SubscribeOnRpcWithParameter<DefinedShapeResponse>(DefineShape);
    }

    private async void CreateField(StartedGameResponse response)
    {
      _mainMenu.gameObject.SetActive(false);
      await _windowManager.CloseWindowsBy<MenuWindow>();
      await _windowManager.OpenWindow<HudWindow>();
      
      _cellCreator.CreateCells(_cells);

      foreach (CellModel cell in _cells)
        _cellVisuals.Add(_cellVisualFactory.Create(cell));
    }

    private void DefineShape(DefinedShapeResponse response)
    {
      _gameInfo.Shape = response.Shape;
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<StartedGameResponse>();
      _networkBus.UnsubscribeFromRpc<DefinedShapeResponse>();
    }
  }
}