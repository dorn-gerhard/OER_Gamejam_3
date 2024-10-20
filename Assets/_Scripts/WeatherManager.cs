using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager instance { get; private set; }

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
    }

    public Vector3Int cell = Vector3Int.zero;

    public void InitRainCells(List<Vector3Int> cells)
    {
        foreach(var cell in cells)
        {
            ApplyEffectToCell(cell);
        }    
    }

    public void ApplyEffectToCell(Vector3Int cell)
    {
        GridManager.instance.SetCellEffect(cell, CellEffect.Rain);
    }
}
