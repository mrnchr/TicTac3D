using CollectiveMind.TicTac3D.Runtime.Shared;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell
{
  [CreateAssetMenu(menuName = CAC.Names.CELL_CONFIG_MENU, fileName = CAC.Names.CELL_CONFIG_FILE)]
  public class CellConfig : ScriptableObject
  {
    public float CellSize = 1;
  }
}