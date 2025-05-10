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

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        //generate random hand
        for (int i = 0; i < cardCount; i++)
        {
            var denominator = (int) Random.Range(2, 5.99f);
            var nominator = Mathf.Min(denominator - 1, (int) Random.Range(2, 5.99f));

            var cardObject = Instantiate(cardPrefab, gameObject.transform);
            cardObject.GetComponent<CardScript>().Setup(nominator, denominator);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}