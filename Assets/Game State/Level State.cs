using UnityEngine;

public class LevelState : MonoBehaviour
{
    public static ChosenGate CurrentlyChosenGate = ChosenGate.None;
    public static PlayerState CurrentPlayerState = PlayerState.MovingNeutral;
    public static int GatesPassedOnCurrentPlatform = 0;
    public static int VotingPower = 10;     // Always start with 10

    public static LevelState Instance;
    public UserInterface userInterface;

    // Singleton
    private void Awake()
    {
        Instance = this;
        UpdateVotingPower();
    }

    public static void UpdateVotingPower()
    {
        Instance.userInterface.UpdateVotingPowerText(VotingPower);
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
    MakingChoice
}

[System.Serializable]
public class PlatformEntry
{
    public Platform platform; // The GameObject reference
    public bool inUse; // Whether the platform is in use
}
