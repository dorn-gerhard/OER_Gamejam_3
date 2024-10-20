using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Grid))]
public class GridManager : MonoBehaviour
{
    public static GridManager instance { get; private set; }
    private Grid _grid;

    [SerializeField]
    private Tilemap _weatherMap;
    [SerializeField]
    private Tile _rainTile;

    private Dictionary<Vector3Int, List<CellEffect>> effectEnhancedCells = new Dictionary<Vector3Int, List<CellEffect>>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        _grid = GetComponent<Grid>();
    }

    public Vector3Int GetCellFromWorldPosition(Vector3 worldPosition)
    {
        return _grid.WorldToCell(worldPosition);
    }

    public Vector3 GetCellCenterFromWorldPosition(Vector3 worldPosition)
    {
        return _grid.GetCellCenterWorld(_grid.WorldToCell(worldPosition));
    }

    public Vector3 GetCellCenter(Vector3Int cell)
    {
        return _grid.CellToWorld(cell);
    }

    public void SetCellEffect(Vector3Int cell, CellEffect effect)
    {
        switch(effect)
        {
            case CellEffect.None:
                break;
            case CellEffect.Rain:
                _weatherMap.SetTile(cell, _rainTile);
                break;
        }

        if(effectEnhancedCells.ContainsKey(cell))
        {
            effectEnhancedCells[cell].Add(effect);
        }
        else
        {
            effectEnhancedCells.Add(cell, new List<CellEffect>{ effect });
        }
    }

    public List<CellEffect> GetCellEffects(Vector3Int cell)
    {
        if(effectEnhancedCells.ContainsKey(cell))
            return effectEnhancedCells[cell];

        return new List<CellEffect>{ CellEffect.None };
    }

    [Button]
    private void ResetCellEffects()
    {
        _weatherMap.ClearAllTiles();
        effectEnhancedCells.Clear();
    }
}
