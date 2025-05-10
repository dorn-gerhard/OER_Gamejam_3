using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int cardCount = 5;

    /// <summary>
    /// Current hand of player.
    /// </summary>
    public readonly List<CardScript> Cards;

    // Start is called before the first frame update
    void Start()
    {
        //generate random hand
        for (int i = 0; i < cardCount; i++)
        {
            var denominator = (int) Random.Range(2, 5.99f);
            var nominator = Mathf.Min(denominator - 1, (int) Random.Range(2, 5.99f));

            //should actually be a GameObject
            Cards.Add(new CardScript(nominator, denominator));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}