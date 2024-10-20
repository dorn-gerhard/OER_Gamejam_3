using TMPro;
using UnityEngine;

public class VotingGate : MonoBehaviour
{
    public TextMeshPro opponentVotingPower;

    public int minVotingPower = 1; // Default minimum value
    public int maxVotingPower = 100; // Default maximum value
    private int _opponentVotingPowerValue;

    private void Start()
    {
        SetOpponentVotingPower(); // Set the opponent's voting power on start
    }

    // Public method to set opponent voting power
    public void SetOpponentVotingPower()
    {
        // Generate a random value within the specified range
        _opponentVotingPowerValue = Random.Range(minVotingPower, maxVotingPower + 1);

        // Update the TextMeshPro text
        opponentVotingPower.text = _opponentVotingPowerValue.ToString();
    }

    // Method to compare voting power and update accordingly
    public void CompareVotingPower()
    {
        int playerVotingPower = LevelState.VotingPower;

        if (playerVotingPower < _opponentVotingPowerValue)
        {
            // Deduct a random percentage from the player (40% to 70%)
            int percentageToDeduct = Random.Range(40, 71); // 40% to 70%
            int deductionAmount = (playerVotingPower * percentageToDeduct) / 100;

            LevelState.VotingPower -= deductionAmount;

            // Ensure VotingPower doesn't go below 1
            if (LevelState.VotingPower < 1)
            {
                LevelState.VotingPower = 1;
            }
        }
        else
        {
            // Add a random percentage to the player from opponent's voting power (20% to 30%)
            int percentageToAdd = Random.Range(20, 31); // 20% to 30%
            int additionAmount = (_opponentVotingPowerValue * percentageToAdd) / 100;

            LevelState.VotingPower += additionAmount;
        }

        // Update the voting power text in the user interface
        LevelState.UpdateVotingPower();
    }
}
