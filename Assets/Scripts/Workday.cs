using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Workday : MonoBehaviour
{
    [SerializeField] private int maximumFunctionPeople = 5;
    [SerializeField] private UnityEvent<int,int> onFunctionPersonCounterUpdated;
    [SerializeField] private UnityEvent onWorkdayComplete;
    
    private int completedFunctionPeople = 0;

    private void Start()
    {
        completedFunctionPeople = 0;
        onFunctionPersonCounterUpdated.Invoke(completedFunctionPeople, maximumFunctionPeople);
    }

    public void IncrementFunctionPeople()
    {
        if (completedFunctionPeople >= maximumFunctionPeople) { return; }
        completedFunctionPeople++;
        onFunctionPersonCounterUpdated.Invoke(completedFunctionPeople, maximumFunctionPeople);
        CheckForCompletion();
    }

    private void CheckForCompletion()
    {
        if (completedFunctionPeople >= maximumFunctionPeople)
        {
            onWorkdayComplete.Invoke();
        }
    }
}
