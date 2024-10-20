using System;
using UnityEngine;

public enum MathModifierType
{
    Addition,
    Subtraction,
    Multiplication,
    Division
}

public class VotingPowerLogic : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Gate currentGate = other.GetComponent<Gate>();

        // Make sure to check if currentGate is not null
        if (currentGate != null)
        {
            UpdateVotingPower(currentGate.mathOperation, currentGate.CalculateMathExpressionValue());
        }
    }

    private void UpdateVotingPower(MathModifierType mathOperationType, float mathExpressionValue)
    {
        // Convert the current voting power to float for calculations
        float currentVotingPower = (float)LevelState.VotingPower;

        // Update voting power based on the math operation type
        switch (mathOperationType)
        {
            case MathModifierType.Addition:
                currentVotingPower += mathExpressionValue; // Add the value
                break;

            case MathModifierType.Subtraction:
                currentVotingPower -= mathExpressionValue; // Subtract the value
                break;

            case MathModifierType.Multiplication:
                currentVotingPower *= mathExpressionValue; // Multiply by the value
                break;

            case MathModifierType.Division:
                // Avoid division by zero
                if (mathExpressionValue != 0)
                {
                    currentVotingPower /= mathExpressionValue; // Divide by the value
                }
                else
                {
                    Debug.LogWarning("Attempted to divide by zero. Voting power remains unchanged.");
                }
                break;

            default:
                Debug.LogWarning("Unknown MathModifierType. Voting power remains unchanged.");
                return; // Exit if the operation type is unknown
        }

        // Round up to the nearest integer
        int newVotingPower = (int)Math.Ceiling(currentVotingPower);

        // Ensure voting power does not go below 1
        LevelState.VotingPower = Mathf.Max(1, newVotingPower);

        LevelState.UpdateVotingPower();

        // Optional: Log the new voting power for debugging purposes
        Debug.Log($"Updated Voting Power: {LevelState.VotingPower}");
    }

}
