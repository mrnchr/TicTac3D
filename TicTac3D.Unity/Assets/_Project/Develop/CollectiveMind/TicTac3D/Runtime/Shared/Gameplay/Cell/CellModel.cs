using System;
using R3;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell
{
  [Serializable]
  public class CellModel
  {
    public Vector3 Index;
    public Vector3 Position;

    public ReactiveProperty<bool> IsHovered = new ReactiveProperty<bool>();
  }
}