using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FunctionDataCollection : MonoBehaviour
{
      
    [SerializeField] private List<FunctionData> easyFunctions;
    [SerializeField] private List<FunctionData> mediumFunctions;
    [SerializeField] private List<FunctionData> hardFunctions;
    
    
    public FunctionData nextData(ComplexityManager.Difficulty difficulty)
    {
        Debug.Log($"Difficulty: {difficulty}");
        switch (difficulty)
        {
            case ComplexityManager.Difficulty.EASY:
                return easyFunctions[Random.Range(0, easyFunctions.Count)];
            case ComplexityManager.Difficulty.MEDIUM:
                return mediumFunctions[Random.Range(0, easyFunctions.Count)];
            case ComplexityManager.Difficulty.HARD:
                return hardFunctions[Random.Range(0, easyFunctions.Count)];
        }
        throw new Exception("This should be unreachable");
     }
}
