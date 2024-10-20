using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public float weight = 0f;
    private List<String> addedIngredients = new List<String>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Increase the weight by 3 when an ingredient is added
        weight += 3;
        Debug.Log("Current weight: " + weight);

        // Get the name of the collided ingredient
        string ingredient = collision.gameObject.name;

        // Check if the object has a Rigidbody2D before modifying it
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        // Find the DragAndDrop script on the collided object and disable it
        DragAndDrop dragAndDrop = collision.gameObject.GetComponent<DragAndDrop>();
        if (dragAndDrop != null)
        {
            dragAndDrop.enabled = false;
        }


        // Add the ingredient to the list if not already added
        /*        if (!addedIngredients.Contains(ingredient))
                {
                    addedIngredients.Add(ingredient);
                    Debug.Log("Ingredient added: " + ingredient);
                }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
