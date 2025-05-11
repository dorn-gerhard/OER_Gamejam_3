using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    public static TimerScript Instance;
    [SerializeField] TMP_Text TimerText;

    private float remainingSeconds;

    // spam card protection: only add time if last add time is longer than a second ago
    private float lastTimeAdd;
    private float totalTime;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        totalTime = 0.0f;
        lastTimeAdd = -1.0f;

        remainingSeconds = 30.0f;
        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;

        if (remainingSeconds > 0.0f)
        {
            remainingSeconds -= Time.deltaTime;
        }
        else
        {
            Highscore.AddHighscore(Player.Instance.Username, Score.Instance.currentScore);
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
        UpdateDisplay();
    }

    public void AddTime(int seconds)
    {
        if (totalTime - lastTimeAdd > 0.8f)
        {
            remainingSeconds += (float) seconds;
        }
        lastTimeAdd = totalTime;
    }

    private void UpdateDisplay()
    {
        var seconds = (int)(remainingSeconds % 60);
        var prependZero = seconds <= 9;

        var minutes = (int)(remainingSeconds / 60);
        TimerText.text = "Time: " + minutes.ToString().Replace("1", "I") + ":" + (prependZero ? "0" : "") + seconds.ToString().Replace("1", "I");
    }

}
