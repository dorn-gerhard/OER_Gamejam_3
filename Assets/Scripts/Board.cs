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
        TableCard.Setup(1, 4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
