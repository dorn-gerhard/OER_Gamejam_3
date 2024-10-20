using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalculateWeight : MonoBehaviour
{
    public float weight = 0f;
    public TMP_Text weightText; // Reference to the Text UI component
    public Slider weightSlider; // Reference to the Slider UI component

    // Dictionary to keep track of the weights of each ingredient
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
            weightText.text = "" + weight.ToString("F2") + " kg";
        }

    }

    // Called when another collider enters the trigger collider attached to the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the IngredientDataHolder component from the collided object
        IngredientDataHolder ingredientData = collision.gameObject.GetComponent<IngredientDataHolder>();
        if (ingredientData != null)
        {
            float ingredientWeight = ingredientData.ingredient.Weight;

            // Add the weight to the total weight
            weight += ingredientWeight/1000f;

            // Store the weight in the dictionary
            ingredientWeights[collision.gameObject] = ingredientWeight;

            // Make the Rigidbody2D dynamic (if needed)
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }

    // Called when another collider exits the trigger collider attached to the object
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the ingredient is in the dictionary
        if (ingredientWeights.ContainsKey(collision.gameObject))
        {
            // Get the stored weight and subtract it from the total weight
            float ingredientWeight = ingredientWeights[collision.gameObject];
            weight -= ingredientWeight;

            // Remove the ingredient from the dictionary
            ingredientWeights.Remove(collision.gameObject);
        }
    }
}
