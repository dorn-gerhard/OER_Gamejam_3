using UnityEngine;

public class LevelState : MonoBehaviour
{
    public static ChosenGate CurrentlyChosenGate = ChosenGate.None;
    public static PlayerState CurrentPlayerState = PlayerState.MovingNeutral;
    public static int GatesPassedOnCurrentPlatform = 0;
    public static int VotingPower = 10;     // Always start with 10

    public static LevelState Instance;
    public UserInterface userInterface;

    public AnimatorManager playerAnimManager;

    // New public variable for the threshold, set in the inspector
    public int votingPowerThreshold = 1000; // Default value, change as needed

    // Singleton
    private void Awake()
    {
        Instance = this;
        ResetLevelState();
        UpdateVotingPower();
    }

    public static void UpdateVotingPower()
    {
        Instance.userInterface.UpdateVotingPowerText(VotingPower);
        Instance.CheckVotingPowerThreshold(); // Check after updating the text
    }

    // Method to check if the voting power has reached the threshold
    private void CheckVotingPowerThreshold()
    {
        if (VotingPower >= votingPowerThreshold)
        {
            Debug.Log("Voting power has reached the threshold!");
            // Delay triggering the GameWon method by 1 second
            Invoke(nameof(GameWon), 3f);
        }
    }

    // Method triggered 1 second after reaching the voting power threshold
    private void GameWon()
    {
        CurrentPlayerState = PlayerState.GameWon;
        playerAnimManager.SetBoolForAll("Is Walking", false);
        userInterface.gameWonWidget.gameObject.SetActive(true);
    }

    public static void ResetLevelState()
    {
        CurrentlyChosenGate = ChosenGate.None;
        CurrentPlayerState = PlayerState.MovingNeutral;
        GatesPassedOnCurrentPlatform = 0;
        VotingPower = 10;  // Reset VotingPower to the initial value
    }

}

public enum ChosenGate
{
    None,
    Left,
    Right
}

public enum PlayerState
{
    MovingNeutral,
    MovingThroughGate,
    MakingChoice,
    GameWon
}

[System.Serializable]
public class PlatformEntry
{
    public Platform platform; // The GameObject reference
    public bool inUse; // Whether the platform is in use
}
