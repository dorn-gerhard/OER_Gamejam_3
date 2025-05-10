using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FunctionAttributesDisplay : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        FunctionAttributes.FunctionAttributesLoaded += UpdateDisplayText;
    }

    private void OnDisable()
    {
        FunctionAttributes.FunctionAttributesLoaded -= UpdateDisplayText;
    }

    private void UpdateDisplayText(string functionAttributes)
    {
        text.text = functionAttributes;
    }
}
