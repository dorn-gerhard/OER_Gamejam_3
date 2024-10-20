using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceSprite : MonoBehaviour
{
    public Sprite pot_filled;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        string ingredient = collision.gameObject.name;
        if (ingredient == "Strawberry")
        {
            // Get the SpriteRenderer component from the parent GameObject
            SpriteRenderer spriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = pot_filled; // Change to the desired sprite
            }
            else
            {
                Debug.LogError("SpriteRenderer component not found on the parent GameObject.");
            }
        }
    }
}
