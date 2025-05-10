using UnityEngine;
using TMPro;

public class ShowFunctionAndAttributes : MonoBehaviour
{
    [SerializeField] private Sprite functionGraph;
    [SerializeField] private SpriteRenderer functionGraphSpot;
    [SerializeField] private string functionAttributes;
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
