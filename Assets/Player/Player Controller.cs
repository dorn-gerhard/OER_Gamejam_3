using UnityEngine;
using UnityEngine.Splines;

public class PlayerController : MonoBehaviour
{
    public AnimatorManager animatorManager;
    public GroundScrolling groundGenerator;

    public Transform cameraTransform;

    public float moveSpeed = 6f; // Speed at which the player moves forward
    public float splineSpeed = 0.25f;

    private Spline _currentSpline; // The current spline the player is following
    private float _splineProgress = 0f; // Current progress along the spline
    private Vector3 _initialPosition; // Store the player's initial position when making a choice
    private float _originalCameraZ; // Store the original camera Z position offset

    private bool _isFirstPlatform = true; // Flag to check if it's the first platform

    private void Start()
    {
        animatorManager.SetBoolForAll("Is Walking", true);
        // Store the original camera Z position offset
        _originalCameraZ = cameraTransform.position.z - transform.position.z;
    }

    void Update()
    {
        if (LevelState.CurrentPlayerState == PlayerState.MakingChoice) return;

        MoveCamera();

        // If the player is moving through a gate, follow the spline
        if (LevelState.CurrentPlayerState == PlayerState.MovingThroughGate)
        {
            FollowSpline();
            return;
        }

        // Regular forward movement
        MoveForward();
    }

    private void MoveForward()
    {
        // Move the player forward every frame
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void MoveCamera()
    {
        // Maintain the camera's original X and Y offsets while updating Z based on the player's position
        Vector3 newCameraPosition = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y,
            transform.position.z + _originalCameraZ
        );

        cameraTransform.position = newCameraPosition;
    }

    public void GateChosen(Spline chosenSpline)
    {
        // Set the current spline based on the chosen gate
        _currentSpline = chosenSpline;

        // Store the player's initial position as an offset
        _initialPosition = transform.position; // Store current position to use as a reference

        // Set player state to moving through the gate
        LevelState.CurrentPlayerState = PlayerState.MovingThroughGate;
        _splineProgress = 0f; // Reset the spline progress

        // Increment the gates passed counter
        LevelState.GatesPassedOnCurrentPlatform += 1;

        // Check if it's the first platform using the flag
        if (_isFirstPlatform)
        {
            // For the first platform, reset after 4 gates
            if (LevelState.GatesPassedOnCurrentPlatform >= 4)
            {
                groundGenerator.SelectRandomUnusedPlatform();
                LevelState.GatesPassedOnCurrentPlatform = 0; // Reset to 0

                // Set the flag to false so that subsequent platforms reset after 5 gates
                _isFirstPlatform = false;
            }
        }
        else
        {
            // For other platforms, reset after 5 gates
            if (LevelState.GatesPassedOnCurrentPlatform >= 5)
            {
                groundGenerator.SelectRandomUnusedPlatform();
                LevelState.GatesPassedOnCurrentPlatform = 0; // Reset to 0
            }
        }
    }

    private void FollowSpline()
    {
        if (_currentSpline == null) return;

        // Determine the speed factor based on the spline progress
        float speedFactor;

        // Slower at the beginning
        if (_splineProgress <= 0.05f)
        {
            speedFactor = Mathf.Lerp(0.05f, splineSpeed, _splineProgress / 0.05f);
        }
        else
        {
            speedFactor = splineSpeed;
        }

        // Increment the spline progress based on the desired speed
        _splineProgress += speedFactor * Time.deltaTime;
        _splineProgress = Mathf.Clamp01(_splineProgress);

        // Get the target position from the spline and apply initial offset
        Vector3 targetPosition = (Vector3)_currentSpline.EvaluatePosition(_splineProgress) + _initialPosition;

        // Interpolate the player's position to the target position for smoother movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f); // Adjust the factor for smoothness

        // Calculate forward direction for rotation, if not at the start
        if (_splineProgress > 0)
        {
            Vector3 previousPosition = (Vector3)_currentSpline.EvaluatePosition(Mathf.Clamp01(_splineProgress - 0.01f)) + _initialPosition;
            Vector3 forward = (targetPosition - previousPosition).normalized;
            transform.rotation = Quaternion.LookRotation(forward);
        }

        // Check if the player has reached the end of the spline
        if (_splineProgress >= 1f)
        {
            LevelState.CurrentPlayerState = PlayerState.MovingNeutral;
        }
    }

}
