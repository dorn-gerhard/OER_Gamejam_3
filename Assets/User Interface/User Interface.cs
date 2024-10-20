using TMPro;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    public TextMeshProUGUI votingPowerText;

    // Method to update the voting power text
    public void UpdateVotingPowerText(int votingPower)
    {
        // Set the text of the voting power UI element
        votingPowerText.text = votingPower.ToString();
    }
}
