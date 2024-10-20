using UnityEngine;

public class VotingCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider has the "Check Votes" tag
        if (other.CompareTag("Check Votes"))
        {
            // Get the VotingGate component from the parent of the collider
            VotingGate votingGate = other.GetComponentInParent<VotingGate>();

            if (votingGate != null)
            {
                votingGate.CompareVotingPower();
            }
            else
            {
                Debug.LogWarning("No VotingGate component found in parent.");
            }
        }
    }
}
