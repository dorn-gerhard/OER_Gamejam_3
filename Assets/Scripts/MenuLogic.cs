using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour
{
    [SerializeField] TMP_Text UsernameEntry;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: load and display highscore
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
        Player.Instance.Username = UsernameEntry.text;
    }
    
    public void ExitGame()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
