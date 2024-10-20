using UnityEngine;
using UnityEngine.Splines;

public class ChoiceMaking : MonoBehaviour
{
    public AnimatorManager animatorManager;
    public PlayerController playerController;

    private bool _isWaitingForChoice = false;
    private ChoicePoint _currentCheckpoint; // Store a reference to the checkpoint

    private void OnTriggerEnter(Collider other)
    {
        if (LevelState.CurrentPlayerState == PlayerState.MovingThroughGate) return;

        // Entered the trigger, set the player state to making a choice
        LevelState.CurrentPlayerState = PlayerState.MakingChoice;
        _isWaitingForChoice = true;

        // Attempt to get the Checkpoint component from the parent of the collided object
        _currentCheckpoint = other.GetComponentInParent<ChoicePoint>();
        if (_currentCheckpoint == null)
        {
            Debug.LogWarning("No Checkpoint component found in the parent of the collided object.");
        }

        animatorManager.SetBoolForAll("Is Walking", false);
    }

    private void Update()
    {
        // Only listen for input when waiting for player choice
        if (_isWaitingForChoice)
        {
            // Check for both LeftArrow and A key for left gate selection
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                LevelState.CurrentlyChosenGate = ChosenGate.Left;
                LevelState.CurrentPlayerState = PlayerState.MovingThroughGate;
                _isWaitingForChoice = false; // Stop waiting for input
                animatorManager.SetBoolForAll("Is Walking", true);

                // Pass the left spline to GateChosen
                if (_currentCheckpoint != null)
                {
                    Spline leftSpline = _currentCheckpoint.GetSpline(useLeftSpline: true);
                    playerController.GateChosen(leftSpline);
                }
            }
            // Check for both RightArrow and D key for right gate selection
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                LevelState.CurrentlyChosenGate = ChosenGate.Right;
                LevelState.CurrentPlayerState = PlayerState.MovingThroughGate;
                _isWaitingForChoice = false; // Stop waiting for input
                animatorManager.SetBoolForAll("Is Walking", true);

                // Pass the right spline to GateChosen
                if (_currentCheckpoint != null)
                {
                    Spline rightSpline = _currentCheckpoint.GetSpline(useLeftSpline: false);
                    playerController.GateChosen(rightSpline);
                }
            }
        }
    }
}
