using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // TODO: load and display highscore
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
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
