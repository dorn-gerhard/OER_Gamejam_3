using UnityEngine;
using NCalc;
using TMPro;

public class Gate : MonoBehaviour
{
    [Header("Parameters to Set")]
    public MathModifierType mathOperation;
    public string mathExpression;

    // Define the range for X
    public int minX = 1;
    public int maxX = 10;

    private int _xValue;

    [Header("Text References")]
    public TextMeshPro xValueText;
    public TextMeshPro operationTypeText;

    // Method to set X to a random value within the defined range
    public void SetRandomXValue()
    {
        _xValue = Random.Range(minX, maxX + 1); // +1 to include maxX
        xValueText.text = "X = " + _xValue;
    }

    public float CalculateMathExpressionValue()
    {
        try
        {
            // Create an NCalc expression from the mathExpression string
            Expression expression = new Expression(mathExpression);

            // Set the variable X in the expression
            expression.Parameters["x"] = _xValue;

            // Evaluate the expression and return the result as a float
            object result = expression.Evaluate();

            // Ensure the result is a valid number and convert it to float
            if (result is double || result is int || result is float)
            {
                return (float)System.Convert.ToDouble(result);
            }
        }
        catch (EvaluationException e)
        {
            Debug.LogError($"Error evaluating expression: {mathExpression}. Error: {e.Message}");
        }

        // Return 1 in case of any errors
        return 1;
    }

    // Awake method to update text based on operation type
    private void Awake()
    {
        UpdateOperationTypeText();
    }

    private void UpdateOperationTypeText()
    {
        switch (mathOperation)
        {
            case MathModifierType.Addition:
                operationTypeText.text = "Plus";
                break;
            case MathModifierType.Subtraction:
                operationTypeText.text = "Minus";
                break;
            case MathModifierType.Multiplication:
                operationTypeText.text = "Multiply";
                break;
            case MathModifierType.Division:
                operationTypeText.text = "Divide";
                break;
            default:
                Debug.LogWarning("Unknown math operation type!");
                break;
        }
    }
}
