using System;
using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Server.Utils;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CollectiveMind.TicTac3D.Runtime.Server.Session
{
  public class BotBrain : IBotBrain
  {
    private readonly List<Vector3> _indices = new List<Vector3>();

    public CellModel GetCellToMove(GameSession session)
    {
      return SelectDangerCell(session) ?? (SelectRandomCell(session, x => x.Index == Vector3.zero)
        ?? SelectRandomCell(session, x => x.Index.magnitude >= Vector3.one.magnitude)
        ?? SelectRandomCell(session, x => x.Index.magnitude <= Vector3.up.magnitude)
        ?? SelectRandomCell(session, x => true));
    }

    private CellModel SelectDangerCell(GameSession session)
    {
      foreach (KeyValuePair<ShapeType, List<CellModel>> item in session.GroupCellsByPlayerShape())
      foreach (List<CellModel> combination in Combinatorics.GetCombinations(item.Value, 2)
        .Select(x => x.ToList()))
      {
        Vector3 direction = combination[1].Index - combination[0].Index;
        if (direction.magnitude > Vector3.one.magnitude)
          direction /= 2;

        _indices.Clear();
        for (int i = -2; i <= 2; i += i == -1 ? 2 : 1)
          _indices.Add(combination[0].Index + direction * i);
        
        _indices.RemoveAll(x => !IsRealIndex(x) || x == combination[1].Index);

        if (_indices.Count == 0)
          continue;

        if (_indices.Count > 1)
          throw new Exception("Unable to determine bot move!");

        CellModel cell = session.Cells.Find(x => x.Index == _indices[0]);
        if (!cell.HasShape())
          return cell;
      }

      return null;
    }

    private CellModel SelectRandomCell(GameSession session, Predicate<CellModel> predicate)
    {
      List<CellModel> freeCells = session.Cells.Where(x => !x.HasShape()).Where(predicate.Invoke).ToList();
      return freeCells.Count > 0 ? freeCells[Random.Range(0, freeCells.Count)] : null;
    }

    private static bool IsRealIndex(Vector3 x)
    {
      return x.magnitude <= Vector3.one.magnitude
        && x.magnitude == new Vector3Int((int)x.x, (int)x.y, (int)x.z).magnitude;
    }
  }
}