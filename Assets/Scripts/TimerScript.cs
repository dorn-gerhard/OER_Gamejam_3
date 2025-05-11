using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TImerScript : MonoBehaviour
{
    [SerializeField] TMP_Text TimerText;

    private float remainingSeconds;
    // Start is called before the first frame update
    void Start()
    {
        remainingSeconds = 90.0f;
        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (remainingSeconds > 0.0f)
        {
            remainingSeconds -= Time.deltaTime;
        }
        UpdateDisplay();
    }
    private void UpdateDisplay()
    {
        var seconds = (int)(remainingSeconds % 60);
        var prependZero = seconds <= 9;

        var minutes = (int)(remainingSeconds / 60);
        TimerText.text = "Time: " + minutes.ToString().Replace("1", "I") + ":" + (prependZero ? "0" : "") + seconds.ToString().Replace("1", "I");
    }

}
