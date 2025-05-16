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
        Player.Username = UsernameEntry.text;

        //fuck Unity's half-baked C# implementation!!
        if (string.IsNullOrWhiteSpace(Player.Username.Replace(((char)8203).ToString(), "")))
        {
            //semi-random default names
            var ran = Random.Range(1, 6);

            Player.Username = ran switch
            {
                1 => "Spongebob",
                2 => "Shrek",
                3 => "Ducktales",
                4 => "Baby Shark",
                _ => "Hello Kitty",
            };
        }
        
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
