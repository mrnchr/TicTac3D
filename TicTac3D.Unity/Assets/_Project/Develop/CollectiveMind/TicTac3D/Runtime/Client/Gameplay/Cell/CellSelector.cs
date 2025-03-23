using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Client.Input;
using CollectiveMind.TicTac3D.Runtime.Client.UI.SetShape;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Cysharp.Threading.Tasks;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell
{
  public class CellSelector : ITickable
  {
    private readonly List<CellModel> _cells;
    private readonly InputProvider _inputProvider;
    private readonly IRpcProvider _rpcProvider;
    private readonly ConfirmationContext _confirmationContext;
    private UniTask _task;

    public CellSelector(List<CellModel> cells,
      InputProvider inputProvider,
      IRpcProvider rpcProvider,
      ConfirmationContext confirmationContext)
    {
      _cells = cells;
      _inputProvider = inputProvider;
      _rpcProvider = rpcProvider;
      _confirmationContext = confirmationContext;
    }

    public void Tick()
    {
      if (_inputProvider.Click && _task.Status != UniTaskStatus.Pending)
      {
        CellModel hoveredCell = _cells.Find(x => x.IsHovered.Value);
        if (hoveredCell != null)
          _task = ReceiveConfirmation(hoveredCell);
      }
    }

    private async UniTask ReceiveConfirmation(CellModel hoveredCell)
    {
      if (await _confirmationContext.Ask())
        _rpcProvider.SendRequest(new SetShapeRequest { CellIndex = hoveredCell.Index });
    }
  }
}