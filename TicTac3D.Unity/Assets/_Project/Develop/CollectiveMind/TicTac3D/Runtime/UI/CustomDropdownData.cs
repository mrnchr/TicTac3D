using CollectiveMind.TicTac3D.Runtime.Utils;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public class CustomDropdownData : MonoBehaviour
  {
    public RectTransform DropdownMenu;
    
    private Canvas _menuCanvas;
    private CanvasGroup _menuCanvasGroup;

    public Canvas MenuCanvas
    {
      get
      {
        if(!_menuCanvas)
          DropdownMenu.OrNull()?.TryGetComponent(out _menuCanvas);
        return _menuCanvas;
      }
    }

    public CanvasGroup MenuMenuCanvasGroup
    {
      get
      {
        if(!_menuCanvasGroup)
          DropdownMenu.OrNull()?.TryGetComponent(out _menuCanvasGroup);
        return _menuCanvasGroup;
      }
    }
  }
}