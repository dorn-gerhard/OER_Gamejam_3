using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DigitalClockDisplay : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateClockDisplay(int currentPeople, int totalPeople)
    {
        text.text = $"{(9 + currentPeople).ToString("D2")}:00";
    }
}
