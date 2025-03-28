﻿using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using Unity.Netcode;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  public struct UpdateShapeResponse : INetworkSerializeByMemcpy
  {
    public Vector3 CellIndex;
    public ShapeType Shape;
  }
}