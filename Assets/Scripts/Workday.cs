using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Workday : MonoBehaviour
{
    [SerializeField] private int maximumFunctionPeople = 5;
    [SerializeField] private UnityEvent<int,int> onFunctionPersonCounterUpdated;
    
    private int completedFunctionPeople = 0;

    private void Start()
    {
        completedFunctionPeople = 0;
        onFunctionPersonCounterUpdated.Invoke(completedFunctionPeople, maximumFunctionPeople);
    }

    public void IncrementFunctionPeople()
    {
        completedFunctionPeople++;
        onFunctionPersonCounterUpdated.Invoke(completedFunctionPeople, maximumFunctionPeople);
    }
}
