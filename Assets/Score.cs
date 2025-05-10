using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TMP_Text ScoreText;
    void Start()
    {
        ScoreText.text = "Score: 0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
