using UnityEngine;
using TMPro;

public class FunctionAttributes : MonoBehaviour
{
    [SerializeField] private Sprite functionGraph;
    [SerializeField] private SpriteRenderer functionGraphSpot;
    [SerializeField] private string functionAttributes;
    [SerializeField] public bool hasCorrectAttributes;
    private TMP_Text attributeTextField;
    void Start()
    {
        attributeTextField = GameObject.FindWithTag("Attributes")?.GetComponent<TMP_Text>();
        ShowQuestion();
    }

    public void ShowQuestion()
    {
        functionGraphSpot.sprite = functionGraph;
        if (attributeTextField != null)
        {
            attributeTextField.text = functionAttributes;
        }
    }



}
