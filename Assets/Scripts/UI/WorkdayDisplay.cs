using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class WorkdayDisplay : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void DisplayFunctionPeopleChecked(int peopleChecked, int maxPeople)
    {
        text.text = $"{peopleChecked}/{maxPeople} function people checked";
    }
}
