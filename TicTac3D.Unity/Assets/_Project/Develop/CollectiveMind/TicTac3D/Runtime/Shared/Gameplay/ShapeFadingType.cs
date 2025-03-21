using System;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  [Flags]
  public enum ShapeFadingType
  {
    None = 0,
    Off = 1,
    Bot = 1 << 1,
    Players = 1 << 2,
    All = Bot | Players
  }
}