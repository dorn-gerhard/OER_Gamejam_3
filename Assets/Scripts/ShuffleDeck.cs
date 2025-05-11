using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleDeck : MonoBehaviour
{
    public void OnCardClicked()
    {
        var cards = GameObject.FindGameObjectsWithTag("Card");

        //replace card (apply cool visual effect?)
        foreach (var cardObject in cards)
            Destroy(cardObject);

        foreach (var _ in cards)
            Player.Instance.GenerateNewCard();
    }
}
