using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionDataCollection : MonoBehaviour
{
    private int currentIndex = -1;
    [SerializeField] private List<FunctionData> functionDatas;

    private void Start()
    {
        if (functionDatas == null || functionDatas.Count == 0)
        {
            Debug.LogWarning("List of function datas is empty!!");
        }
    }
    public FunctionData nextData()
    {

        if (currentIndex + 2 > functionDatas.Count) //Index out of Scope
        {
            return functionDatas[0];
        }
        else
        {
            currentIndex++;
            return functionDatas[currentIndex];
            
        }
    }
}
