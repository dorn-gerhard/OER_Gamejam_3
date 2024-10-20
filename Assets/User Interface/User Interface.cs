using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;  // Import SceneManager for restarting scenes

public class UserInterface : MonoBehaviour
{
    public TextMeshProUGUI votingPowerText;
    public GameObject gameWonWidget;
    public TextMeshProUGUI gameStartInstructions;

    private void Start()
    {
        if (gameStartInstructions != null)
        {
            gameStartInstructions.text = "Get " + LevelState.Instance.votingPowerThreshold + " Votes\nto Win!";
            Invoke(nameof(DisableGameStartInstructions), 10f);  // Disable instructions after 10 seconds
        }
    }

    // Method to update the voting power text
    public void UpdateVotingPowerText(int votingPower)
    {
        // Set the text of the voting power UI element
        votingPowerText.text = votingPower.ToString();
    }

    private void Update()
    {
        // Check if the Escape key was pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseApplication();
        }
    }

    // Method to close the application
    private void CloseApplication()
    {
        Application.Quit();
    }

    // Method to disable the gameStartInstructions after 5 seconds
    private void DisableGameStartInstructions()
    {
        if (gameStartInstructions != null)
        {
            gameStartInstructions.gameObject.SetActive(false); // Disable the UI element
        }
    }

    // Method to restart the current level
    public void RestartLevel()
    {
        // Get the currently active scene and reload it
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}