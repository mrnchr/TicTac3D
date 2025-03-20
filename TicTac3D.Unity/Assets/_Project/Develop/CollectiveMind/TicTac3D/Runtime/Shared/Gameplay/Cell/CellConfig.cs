using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell
{
  [CreateAssetMenu(menuName = CAC.Names.CELL_CONFIG_MENU, fileName = CAC.Names.CELL_CONFIG_FILE)]
  public class CellConfig : ScriptableObject
  {
    public float CellSize = 1;

    public float MaxRaycastDistance = 100;
    
    public Color DefaultColor;
    public Color HoveredColor;
  }
}