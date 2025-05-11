using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int currentScore;

    [SerializeField] TMP_Text ScoreText;
    [SerializeField] int ScoreMultiplier = 1;
    [SerializeField] int PenaltyMultiplier = 1;

    private AudioSource audioSource;
    public AudioClip failClip;
    public AudioClip successClip;

    public static Score Instance;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        Instance = this;
        Update();
    }

    public void Add(int points)
    {
        audioSource.PlayOneShot(successClip, .6f);

        currentScore += ScoreMultiplier * points;
        Update();
    }

    public void Punish(int points)
    {
        audioSource.PlayOneShot(failClip);

        currentScore -= PenaltyMultiplier * points;
        Update();
    }

    public void Update()
    {
        //not localized :(
        ScoreText.text = $"Score: {currentScore}";
    }
}
