using System;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  [Flags]
  public enum ShapeFadingType
  {
    None = 0,
    Bot = 1,
    Players = 2,
    All = Bot | Players
  }
}