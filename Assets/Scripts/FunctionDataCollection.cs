using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionDataCollection : MonoBehaviour
{
    public enum Difficulty
    {
        EASY,
        MEDIUM,
        HARD
    }
    
    private int currentIndex = -1;
    [SerializeField] private List<FunctionData> easyFunctions;
    [SerializeField] private List<FunctionData> mediumFunctions;
    [SerializeField] private List<FunctionData> hardFunctions;
    
    
    public FunctionData nextData(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.EASY:
                break;
            case Difficulty.MEDIUM:
                break;
            case Difficulty.HARD:
                break;
        }

        if (currentIndex + 2 > easyFunctions.Count) //Index out of Scope
        {
            return easyFunctions[0];
        }
        else
        {
            currentIndex++;
            return easyFunctions[currentIndex];
        }
    }
}
