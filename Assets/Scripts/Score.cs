using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TMP_Text ScoreText;

    public int ScoreNumber;
    void Start()
    {
        ScoreNumber = 10;
        ScoreText.text = "Score: " + ScoreNumber.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
