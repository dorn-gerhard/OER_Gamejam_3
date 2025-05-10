using UnityEngine;
using TMPro;

public class FunctionAttributes : MonoBehaviour
{
    private FunctionData functionData;
    [SerializeField] private SpriteRenderer functionGraphSpot;

    public delegate void OnFunctionAttributesLoaded(string functionAttributes);
    public static event OnFunctionAttributesLoaded FunctionAttributesLoaded;


    public void UpdateFunctionData(FunctionData newFunctionData)
    {
        functionData = newFunctionData;
        ShowQuestion();
    }
    private void ShowQuestion()
    {
        functionGraphSpot.sprite = functionData.functionGraph;
        FunctionAttributesLoaded?.Invoke(functionData.functionAttributes);
    }

    public bool hasCorrectAttributes()
    {
        return functionData.is_correct;
    }

}
