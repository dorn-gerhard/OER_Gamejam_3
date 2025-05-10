using UnityEngine;
using TMPro;

public class FunctionAttributes : MonoBehaviour
{
    private FunctionData functionData;
    [SerializeField] private SpriteRenderer functionGraphSpot;

    private TMP_Text attributeTextField;
    void Start()
    {
        attributeTextField = GameObject.FindWithTag("Attributes")?.GetComponent<TMP_Text>();
        
    }

    public void UpdateFunctionData(FunctionData newFunctionData)
    {
        functionData = newFunctionData;
        ShowQuestion();
    }

    public void ShowQuestion()
    {
        functionGraphSpot.sprite = functionData.functionGraph;
        if (attributeTextField != null)
        {
            attributeTextField.text = functionData.functionAttributes;
        }
    }

    public bool hasCorrectAttributes()
    {
        return functionData.is_correct;
    }

}
