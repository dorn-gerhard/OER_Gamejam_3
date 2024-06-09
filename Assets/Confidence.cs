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

    public void UpdateConfidenceUI()
    {
        fillBar.fillAmount = confidence / 100;
        procentageText.text = confidence.ToString("F0") + "%";
    }
}
