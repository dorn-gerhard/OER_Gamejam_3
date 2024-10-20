using System.Collections.Generic;
using UnityEngine;

public class GroundScrolling : MonoBehaviour
{
    public List<Platform> groundPlatformVariants; // List of ground platform entries
    public float spawnOffset = 100f; // Distance in Z to spawn the next platform
    private List<PlatformEntry> _platformEntries = new List<PlatformEntry>(); // List to hold PlatformEntry objects
    private Queue<PlatformEntry> _activePlatformsQueue = new Queue<PlatformEntry>(); // Queue for platforms in use
    public int maxActivePlatforms = 3; // Maximum number of active platforms at any time
    private float _lastZPosition; // Tracks the Z position of the last placed platform
    private bool _isFirstPlatform = true; // Flag to track if it's the first platform

    private void Awake()
    {
        InitializePlatformEntries();
        SelectInitialPlatforms();
    }

    private void InitializePlatformEntries()
    {
        foreach (Platform platform in groundPlatformVariants)
        {
            if (platform == null)
            {
                Debug.LogError("Found a null platform in groundPlatformVariants!");
                continue; // Skip this iteration if platform is null
            }
            _platformEntries.Add(new PlatformEntry { platform = platform, inUse = false });
        }
    }

    private void SelectInitialPlatforms()
    {
        if (_platformEntries.Count < 2)
        {
            Debug.LogWarning("Not enough platforms to select from!");
            return;
        }

        PlatformEntry firstPlatform = SelectRandomUnusedPlatform(40f);

        if (firstPlatform != null && firstPlatform.platform != null)
        {
            Platform platformComponent = firstPlatform.platform.GetComponent<Platform>();
            if (platformComponent != null && platformComponent.firstChoicePoint != null)
            {
                platformComponent.firstChoicePoint.gameObject.SetActive(false); // Disable the first gate
            }
        }

        SelectRandomUnusedPlatform(150f);
        _lastZPosition = 150f;
    }

    public PlatformEntry SelectRandomUnusedPlatform(float? zPosition = null)
    {
        List<PlatformEntry> unusedPlatforms = _platformEntries.FindAll(p => !p.inUse);
        if (unusedPlatforms.Count == 0)
        {
            Debug.LogWarning("No unused platforms available!");
            return null;
        }

        PlatformEntry selectedPlatform = unusedPlatforms[Random.Range(0, unusedPlatforms.Count)];
        float newZPosition = zPosition ?? (_lastZPosition + spawnOffset);
        _lastZPosition = newZPosition;

        ActivatePlatform(selectedPlatform, newZPosition);
        return selectedPlatform;
    }

    private void ActivatePlatform(PlatformEntry platformEntry, float zPosition)
    {
        if (platformEntry == null || platformEntry.platform == null)
        {
            Debug.LogError("Attempted to activate a null platform entry or platform!");
            return;
        }

        platformEntry.inUse = true; // Mark as in use
        platformEntry.platform.transform.position = new Vector3(0, 0, zPosition); // Set position
        platformEntry.platform.transform.rotation = Quaternion.identity; // Reset rotation

        Platform platformComponent = platformEntry.platform.GetComponent<Platform>();
        if (platformComponent == null)
        {
            Debug.LogError($"The platform {platformEntry.platform.name} does not have a Platform component!");
            return;
        }

        if (platformComponent.firstChoicePoint == null)
        {
            Debug.LogWarning($"The platform {platformEntry.platform.name} does not have a valid firstChoicePoint.");
        }
        else if (_isFirstPlatform)
        {
            _isFirstPlatform = false; // After this, the first platform is no longer considered
        }
        else
        {
            platformComponent.firstChoicePoint.gameObject.SetActive(true);
        }

        // Call the new method to set random X values for gates
        SetRandomXValuesForGates(platformComponent);

        // Set random opponent voting power
        SetRandomOpponentVotingPower(platformComponent);

        _activePlatformsQueue.Enqueue(platformEntry);

        if (_activePlatformsQueue.Count > maxActivePlatforms)
        {
            PlatformEntry oldestPlatform = _activePlatformsQueue.Dequeue();
            DeactivatePlatform(oldestPlatform);
        }
    }

    // New method to set random X values for gates
    private void SetRandomXValuesForGates(Platform platform)
    {
        foreach (ChoicePoint choicePoint in platform.platformChoicePoints)
        {
            // Set random X value for the left gate if it exists
            if (choicePoint.leftGate != null)
            {
                int randomXValue = Random.Range(choicePoint.leftGate.minX, choicePoint.leftGate.maxX + 1);
                choicePoint.leftGate.SetRandomXValue();
            }

            // Set random X value for the right gate if it exists
            if (choicePoint.rightGate != null)
            {
                int randomXValue = Random.Range(choicePoint.rightGate.minX, choicePoint.rightGate.maxX + 1);
                choicePoint.rightGate.SetRandomXValue();
            }
        }
    }

    // Method to set random opponent voting power
    private void SetRandomOpponentVotingPower(Platform platform)
    {
        VotingGate votingGate = platform.GetComponent<VotingGate>();
        if (votingGate != null)
        {
            votingGate.SetOpponentVotingPower();
        }
        else
        {
            Debug.LogWarning($"No VotingGate component found on platform: {platform.name}");
        }
    }

    private void DeactivatePlatform(PlatformEntry platformEntry)
    {
        platformEntry.inUse = false; // Mark as not in use
        platformEntry.platform.transform.position = new Vector3(0, -1000, 0); // Move it off-screen
    }
}
