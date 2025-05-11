using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    public CardScript TableCard { get; private set; }

    [SerializeField]
    private GameObject cardPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        var cardObject = Instantiate(cardPrefab, gameObject.transform);
        TableCard = cardObject.GetComponent<CardScript>();

        //set random start value
        
        var denominator = BoardCardDenominator();
        var nominator = 1;

        TableCard.Setup(nominator, denominator);
    }

    int BoardCardDenominator()
    {
        return (int)Random.Range(2, 5.99f);
    }
}
