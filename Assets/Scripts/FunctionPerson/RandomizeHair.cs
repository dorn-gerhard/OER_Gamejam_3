using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomizeHair : MonoBehaviour
{
    [SerializeField] SpriteRenderer hairSprite;
    [SerializeField] List<Sprite> hairTypes;
    [SerializeField] List<Color> hairColors;
    void Start()
    {
        
        int typeIndex = Random.Range(0, hairTypes.Count);
        int colorIndex = Random.Range(0, hairColors.Count);
        hairSprite.sprite = hairTypes[typeIndex];
        hairSprite.color = hairColors[colorIndex];
    }

}
