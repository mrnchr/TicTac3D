using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  [CreateAssetMenu(menuName = CAC.Names.CELL_CONFIG_MENU, fileName = "CellConfig")]
  public class CellConfig : ScriptableObject
  {
    public float CellSize = 1;

    public float MaxRaycastDistance = 100;
    
    public Color DefaultColor;
    public Color HoveredColor;
  }
}