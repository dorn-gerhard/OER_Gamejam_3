using UnityEngine;
using UnityEngine.Events;

public class Workday : MonoBehaviour
{

    public struct WorkdayReport
    {
        public int CorrectPeople;
        public int MaximumPeople;

        public WorkdayReport(int correctPeople, int maximumPeople)
        {
            CorrectPeople = correctPeople;
            MaximumPeople = maximumPeople;
        }
    }
    
    [SerializeField] private int maximumFunctionPeople = 5;
    [SerializeField] private UnityEvent onWorkdayStarted;
    [SerializeField] private UnityEvent<int,int> onFunctionPersonCounterUpdated;
    [SerializeField] private UnityEvent<WorkdayReport> onWorkdayComplete;
    
    private int completedFunctionPeople = 0;
    private int correctFunctionPeople = 0;

    private void Start()
    {
        StartWorkday();
    }

    public void StartWorkday()
    {
        completedFunctionPeople = 0;
        correctFunctionPeople = 0;
        onFunctionPersonCounterUpdated.Invoke(completedFunctionPeople, maximumFunctionPeople);
        onWorkdayStarted.Invoke();
    }

    public void AnsweredCorrect()
    {
        correctFunctionPeople++;
    }

    public void AnsweredIncorrect()
    {
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
            onWorkdayComplete.Invoke(new WorkdayReport(correctFunctionPeople, completedFunctionPeople));
        }
    }
}
