using UnityEngine.Events;
using UnityEngine.UI;

namespace CollectiveMind.TicTac3D.Runtime.Client.WindowManagement
{
  public static class ButtonExtensions
  {
    public static void AddListener(this Button button, UnityAction callback)
    {
      if (!button)
        return;
      
      button.onClick.AddListener(callback);
    }
    
    public static void RemoveListener(this Button button, UnityAction callback)
    {
      if (!button)
        return;
      
      button.onClick.RemoveListener(callback);
    }
  }
}