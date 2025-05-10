using UnityEngine;
using TMPro;

public class FunctionAttributes : MonoBehaviour
{
    private FunctionData functionData;
    [SerializeField] private SpriteRenderer functionGraphSpot;
    [SerializeField] private string functionAttributes;
    [SerializeField] public bool hasCorrectAttributes;
    public delegate void OnFunctionAttributesLoaded(string functionAttributes);
    public static event OnFunctionAttributesLoaded FunctionAttributesLoaded;
    void Start()
    {
        attributeTextField = GameObject.FindWithTag("Attributes")?.GetComponent<TMP_Text>();
        
    }

    public void UpdateFunctionData(FunctionData newFunctionData)
    {
        functionData = newFunctionData;
        ShowQuestion();
    }
    private void ShowQuestion()
    {
        functionGraphSpot.sprite = functionGraph;
        FunctionAttributesLoaded?.Invoke(functionAttributes);
    }

    public bool hasCorrectAttributes()
    {
        return functionData.is_correct;
    }

}
