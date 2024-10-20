using UnityEngine;

public class CrowdVisuals : MonoBehaviour
{
    private CharacterTier _activeCharacterTier; // Currently active character tier
    private bool _isMale; // Gender state of the active character
    private int _currentVotingPower; // Current voting power affecting the character's tier

    // This should be assigned through the inspector or code before the game starts
    public TierList tierList;

    private void Start()
    {
        // Determine initial gender and tier for the character
        _isMale = Random.Range(0, 2) == 0; // Randomly set gender
        _activeCharacterTier = tierList.characterTiers[0]; // Start with the first tier
        ActivateCharacter(_activeCharacterTier, _isMale);
    }

    public void UpdateVotingPower(int newVotingPower)
    {
        if (newVotingPower != _currentVotingPower)
        {
            int difference = newVotingPower - _currentVotingPower;
            HandleCharacterChanges(ref difference);
            _currentVotingPower = newVotingPower; // Update current voting power
        }
    }

    private void HandleCharacterChanges(ref int difference)
    {
        if (difference > 0)
        {
            // Attempt to upgrade character
            UpgradeCharacter(ref difference);
        }
        else if (difference < 0)
        {
            // Attempt to downgrade character
            DowngradeCharacter(ref difference);
        }
    }

    private void UpgradeCharacter(ref int overflow)
    {
        // Check for upgrades based on the current tier's min and max values
        for (int i = 0; i < tierList.characterTiers.Length; i++)
        {
            CharacterTier tier = tierList.characterTiers[i];
            if (tier == _activeCharacterTier) // Current tier
            {
                if (overflow >= tier.scoreMin && overflow <= tier.scoreMax)
                {
                    return; // No upgrade needed
                }
                else if (overflow > tier.scoreMax && i < tierList.characterTiers.Length - 1)
                {
                    // Upgrade to the next tier
                    _activeCharacterTier = tierList.characterTiers[i + 1];
                    overflow -= tier.scoreMax; // Deduct the max score of the current tier
                    ActivateCharacter(_activeCharacterTier, _isMale);
                    return; // Exit after upgrading
                }
            }
        }
    }

    private void DowngradeCharacter(ref int deficit)
    {
        // Check for downgrades based on the current tier's min and max values
        for (int i = 0; i < tierList.characterTiers.Length; i++)
        {
            CharacterTier tier = tierList.characterTiers[i];
            if (tier == _activeCharacterTier) // Current tier
            {
                if (deficit >= tier.scoreMin)
                {
                    if (i > 0)
                    {
                        // Downgrade to the previous tier
                        _activeCharacterTier = tierList.characterTiers[i - 1];
                        deficit -= _activeCharacterTier.scoreMax; // Deduct the max score of the new current tier
                        ActivateCharacter(_activeCharacterTier, _isMale);
                    }
                    return; // Exit after downgrading
                }
            }
        }
    }

    private void ActivateCharacter(CharacterTier tier, bool isMale)
    {
        // Deactivate all characters and activate the current one
        foreach (var characterTier in tierList.characterTiers)
        {
            if (characterTier.characterFemale != null) characterTier.characterFemale.SetActive(false);
            if (characterTier.characterMale != null) characterTier.characterMale.SetActive(false);
        }

        // Activate the current character
        if (tier.characterFemale != null) tier.characterFemale.SetActive(isMale == false);
        if (tier.characterMale != null) tier.characterMale.SetActive(isMale);
    }
}
