using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Client.Input;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell
{
  public class CellSelector : ITickable
  {
    private readonly List<CellModel> _cells;
    private readonly InputProvider _inputProvider;
    private readonly IRpcProvider _rpcProvider;

    public CellSelector(List<CellModel> cells, InputProvider inputProvider, IRpcProvider rpcProvider)
    {
      _cells = cells;
      _inputProvider = inputProvider;
      _rpcProvider = rpcProvider;
    }

    public void Tick()
    {
      if (_inputProvider.Click)
      {
        CellModel hoveredCell = _cells.Find(x => x.IsHovered.Value);
        if (hoveredCell != null)
          _rpcProvider.SendRequest(new SetShapeRequest { CellIndex = hoveredCell.Index });
      }
    }
  }
}