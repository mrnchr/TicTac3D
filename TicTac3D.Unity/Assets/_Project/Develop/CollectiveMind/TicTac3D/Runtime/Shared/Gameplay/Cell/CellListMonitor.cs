﻿using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell
{
  public class CellListMonitor : MonoBehaviour
  {
    [SerializeField] private List<CellModel> _cells;
    
    [Inject]
    public void Construct(List<CellModel> cells)
    {
      _cells = cells;
    }
  }
}