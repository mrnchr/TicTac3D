using UnityEngine.Events;
using UnityEngine.UI;

namespace CollectiveMind.TicTac3D.Runtime.WindowManagement
{
  public static class UIExtensions
  {
    public static void AddListener(this Button button, UnityAction callback)
    {
      if (button)
        button.onClick.AddListener(callback);
    }

    public static void RemoveListener(this Button button, UnityAction callback)
    {
      if (button)
        button.onClick.RemoveListener(callback);
    }

    public static void AddListener(this Slider slider, UnityAction<float> callback)
    {
      if (slider)
        slider.onValueChanged.AddListener(callback);
    }

    public static void RemoveListener(this Slider slider, UnityAction<float> callback)
    {
      if (slider)
        slider.onValueChanged.RemoveListener(callback);
    }
  }
}