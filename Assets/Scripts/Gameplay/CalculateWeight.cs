using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalculateWeight : MonoBehaviour
{
    public float weight = 0f;
    public TMP_Text weightText; // Reference to the Text UI component
    public Slider weightSlider; // Reference to the Slider UI component
    private List<BasicIngredient> collidedIngredients = new List<BasicIngredient>();

    void Update()
    {
        // Update the total weight and UI elements every frame
        //weight = GetTotalWeight();
        UpdateUI();
    }

    // Function to calculate the total weight of all currently collided ingredients
   /* public float GetTotalWeight()
    {
        //float totalWeight = 0f;


        // Iterate through all currently collided ingredients
        foreach (BasicIngredient ingredient in collidedIngredients)
        {
            if (ingredient != null)
            {
                // Add the weight of the ingredient to the total weight
                totalWeight += ingredient.Weight;
            }
        }

        return totalWeight;
    }*/

    // Update the UI elements (Text and Slider) with the current weight value
    private void UpdateUI()
    {
        if (weightText != null)
        {
            weightText.text = "" + weight.ToString("F2") + " kg";
        }

        if (weightSlider != null)
        {
            // Assuming the slider's max value is set appropriately
            weightSlider.value = weight;
        }
    }

    // Called when another collider enters the trigger collider attached to the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("ONTRIGGERENTER +");
        weight += 3;
        Debug.Log("weight" + weight);

        //collidedIngredients.Add(ingredient);
        // Check if the collided object has an Ingredient component
        /*        BasicIngredient ingredient = collision.GetComponent<BasicIngredient>();
                if (ingredient != null && !collidedIngredients.Contains(ingredient))
                {
                    // Add the ingredient to the list of collided ingredients
                    collidedIngredients.Add(ingredient);
                }*/
    }

    // Called when another collider exits the trigger collider attached to the object
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the object leaving the trigger has an Ingredient component
        /*        BasicIngredient ingredient = collision.GetComponent<BasicIngredient>();
                if (ingredient != null && collidedIngredients.Contains(ingredient))
                {
                    // Remove the ingredient from the list of collided ingredients
                    collidedIngredients.Remove(ingredient);
                }*/
        weight -= 3;

    }
}
