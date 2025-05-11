using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class Calendar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI monthText;
    [SerializeField] private TextMeshProUGUI dayText;
    
    void Start()
    {
        CultureInfo ci = new CultureInfo("en-US");
        DateTime today = DateTime.Today;
        monthText.text = today.ToString("MMMM", ci);
        dayText.text = today.ToString("dd", ci);
        
    }


}
