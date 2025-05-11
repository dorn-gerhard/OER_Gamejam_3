using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FunctionDataCollection : MonoBehaviour
{
    public enum Difficulty
    {
        EASY,
        MEDIUM,
        HARD
    }
    
    [SerializeField] private List<FunctionData> easyFunctions;
    [SerializeField] private List<FunctionData> mediumFunctions;
    [SerializeField] private List<FunctionData> hardFunctions;
    
    
    public FunctionData nextData(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.EASY:
                return easyFunctions[Random.Range(0, easyFunctions.Count)];
            case Difficulty.MEDIUM:
                return mediumFunctions[Random.Range(0, easyFunctions.Count)];
            case Difficulty.HARD:
                return hardFunctions[Random.Range(0, easyFunctions.Count)];
        }
        throw new Exception("This should be unreachable");
     }
}
