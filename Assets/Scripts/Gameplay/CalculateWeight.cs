using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalculateWeight : MonoBehaviour
{
    public float weight = 0f;
    public TMP_Text weightText; // Reference to the Text UI component
    public Slider weightSlider; // Reference to the Slider UI component

    // Dictionary to keep track of the weights of each ingredient (stored in kilograms)
    private Dictionary<GameObject, float> ingredientWeights = new Dictionary<GameObject, float>();

    void Update()
    {
        // Update the total weight and UI elements every frame
        UpdateUI();
    }

    // Update the UI elements (Text and Slider) with the current weight value
    private void UpdateUI()
    {
        if (weightText != null)
        {
            weightText.text = weight.ToString("F3") + " kg";
        }
    }

    // Called when another collider enters the trigger collider attached to the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the IngredientDataHolder component from the collided object
        IngredientDataHolder ingredientData = collision.gameObject.GetComponent<IngredientDataHolder>();
        if (ingredientData != null)
        {
            float ingredientWeightInGrams = ingredientData.ingredient.Weight;

            // Convert the weight to kilograms
            float ingredientWeightInKg = ingredientWeightInGrams / 1000f;

            // Add the weight to the total weight
            weight += ingredientWeightInKg;

            // Store the weight in the dictionary (in kilograms)
            ingredientWeights[collision.gameObject] = ingredientWeightInKg;

            // Make the Rigidbody2D dynamic (if needed)
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }

            Debug.Log("Added weight: " + ingredientWeightInKg + " kg, Total weight: " + weight + " kg");
        }
    }

    // Called when another collider exits the trigger collider attached to the object
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the ingredient is in the dictionary
        if (ingredientWeights.ContainsKey(collision.gameObject))
        {
            // Get the stored weight in kilograms and subtract it from the total weight
            float ingredientWeightInKg = ingredientWeights[collision.gameObject];
            weight -= ingredientWeightInKg;

            // Remove the ingredient from the dictionary
            ingredientWeights.Remove(collision.gameObject);

            Debug.Log("Removed weight: " + ingredientWeightInKg + " kg, Total weight: " + weight + " kg");
        }
    }
}
