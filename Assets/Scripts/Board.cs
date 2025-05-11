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
        cardObject.tag = "Untagged"; //hack to unregister this as player card
        TableCard = cardObject.GetComponent<CardScript>();

        //random fraction
        TableCard.Setup(1, (int)Random.Range(2, 5.99f));
    }
}
