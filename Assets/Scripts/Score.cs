using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    private int currentScore;

    [SerializeField] TMP_Text ScoreText;
    [SerializeField] int ScoreMultiplier = 1;
    [SerializeField] int PenaltyMultiplier = 1;

    public static Score Instance;

    void Start()
    {
        Instance = this;
        Update();
    }

    public void Add(int points)
    {
        currentScore += ScoreMultiplier * points;
        Update();
    }

    public void Punish(int points)
    {
        currentScore -= PenaltyMultiplier * points;
        Update();
    }

    public void Update()
    {
        //not localized :(
        ScoreText.text = $"Score: {currentScore}";
    }
}
