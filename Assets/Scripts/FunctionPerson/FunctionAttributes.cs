using UnityEngine;
using TMPro;

public class FunctionAttributes : MonoBehaviour
{
    [SerializeField] private Sprite functionGraph;
    [SerializeField] private SpriteRenderer functionGraphSpot;
    [SerializeField] private string functionAttributes;
    [SerializeField] public bool hasCorrectAttributes;
    public delegate void OnFunctionAttributesLoaded(string functionAttributes);
    public static event OnFunctionAttributesLoaded FunctionAttributesLoaded;
    void Start()
    {
        ShowQuestion();
    }
    private void ShowQuestion()
    {
        functionGraphSpot.sprite = functionGraph;
        FunctionAttributesLoaded?.Invoke(functionAttributes);
    }



}
