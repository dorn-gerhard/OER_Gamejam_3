using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapFractions : MonoBehaviour
{
    public void OnCardClicked()
    {
        var cards = GameObject.FindGameObjectsWithTag("Card");

        foreach (var cardObject in cards)
            cardObject.GetComponent<CardScript>().Swap();
    }
}
