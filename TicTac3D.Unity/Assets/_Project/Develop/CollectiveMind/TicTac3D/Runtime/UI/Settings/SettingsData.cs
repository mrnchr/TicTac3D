using System;
using R3;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  [Serializable]
  public class SettingsData
  {
    public SerializableReactiveProperty<float> SoundVolume = new SerializableReactiveProperty<float>();
    public SerializableReactiveProperty<float> MusicVolume = new SerializableReactiveProperty<float>();
    public SerializableReactiveProperty<float> MouseSensitivity = new SerializableReactiveProperty<float>();

    public void Copy(SettingsData from)
    {
      SoundVolume.Value = from.SoundVolume.Value;
      MusicVolume.Value = from.MusicVolume.Value;
      MouseSensitivity.Value = from.MouseSensitivity.Value;
    }
  }
}