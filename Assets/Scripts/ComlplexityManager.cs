using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ComplexityManager : MonoBehaviour
{
    public static ComplexityManager Instance { get; private set; }

    public enum Difficulty
    {
       EASY,
       MEDIUM,
       HARD
    } // stores which complexity was chosen

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object when changing scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates if one already exists
        }
    }
}