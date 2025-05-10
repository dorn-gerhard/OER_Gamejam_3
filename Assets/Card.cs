using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TMP_Text CardNumerator;
    [SerializeField] TMP_Text CardDenominator;
    void Start()
    {
        CardNumerator.text = "1";
        CardDenominator.text = "3";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
