using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score Instance;

    // Start is called before the first frame update
    [SerializeField] TMP_Text ScoreText;

    int currentScore;

    void Start()
    {
        Instance = this;
        Update();
    }

    public void Add(int points)
    {
        currentScore += points;
        Update();
    }

    public void Update()
    {
        ScoreText.text = $"Score: {currentScore}";
    }
}
