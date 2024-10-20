using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public float weight = 0f;
    private List<String> addedIngredients = new List<String>();
    private List<float> weightIngredient = new List<float>();

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // Get the name of the collided ingredient
        string ingredient = collision.gameObject.name;
        addedIngredients.Add(ingredient);

         float weight = collision.gameObject.GetComponent<IngredientDataHolder>().ingredient.Weight; 
         weightIngredient.Add(weight);

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

    }

 

    public Dictionary<string, float> CalculateWeightOfEachIngredient()
    {
        if (addedIngredients.Count != weightIngredient.Count)
        {
            Debug.LogError("The number of ingredients and weights must match.");
            return null;
        }

        Dictionary<string, float> ingredientWeights = new Dictionary<string, float>();

        // Loop through each ingredient and its corresponding weight
        for (int i = 0; i < addedIngredients.Count; i++)
        {
            string ingredient = addedIngredients[i];
            float weight = weightIngredient[i];

            // If the ingredient already exists in the dictionary, add the weight
            if (ingredientWeights.ContainsKey(ingredient))
            {
                ingredientWeights[ingredient] += weight;
            }
            else
            {
                // If the ingredient does not exist, add it with the current weight
                ingredientWeights[ingredient] = weight;
            }
        }
        return ingredientWeights;
    }


}
