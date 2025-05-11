using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField]
    private int cardCount = 5;

    [SerializeField]
    private GameObject cardPrefab;

    public string Username { get; internal set; } = "";

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        //generate random hand
        for (int i = 0; i < cardCount; i++)
        {
            GenerateNewCard();
        }
    }

    //dirty hack for cards to instantiate themselves (bad habit)
    internal void GenerateNewCard()
    {
        var denominator = (int)Random.Range(2, 5.99f);
        var nominator = Mathf.Min(denominator - 1, (int)Random.Range(1, 4.99f));

        var cardObject = Instantiate(cardPrefab, gameObject.transform);
        cardObject.GetComponent<CardScript>().Setup(nominator, denominator);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}