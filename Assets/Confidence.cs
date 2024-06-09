using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Confidence : MonoBehaviour
{
    public Image functionExampleImage;
    public float confidence = 0;
    public Image fillBar;
    public TMP_Text procentageText;

    public float decayDelay = 3;
    public float decayAmount = 10;
    float timeSinceLastDelay = 0;

    public void UpdateConfidenceUI()
    {
        fillBar.fillAmount = confidence / 100;
        procentageText.text = confidence.ToString("F0") + "%";
    }

    private void Update()
    {
        Decay();
    }

    void Decay()
    {
        if (gameObject.activeSelf && gameObject.GetComponent<CanvasGroup>().alpha < 1)
        {
            timeSinceLastDelay += Time.deltaTime;
            if (timeSinceLastDelay >= decayDelay)
            {
                confidence -= decayAmount;
                UpdateConfidenceUI();
                timeSinceLastDelay = 0;
            }            
        }
    }

    //void ChangeConfidence
    //{

    //}
}
