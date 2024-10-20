using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Pot potScript; // Reference to the Pot script
    public Sprite chefHappy;
    public Sprite chefSad;
    public Image chefImage; // Reference to the UI.Image component


    // Start is called before the first frame update
    void Start()
    {
        // Find the Pot GameObject in the scene
        GameObject potObject = GameObject.Find("Pot"); 
        if (potObject != null)
        {
            // Get the Pot script component
            potScript = potObject.GetComponent<Pot>();
        }
        else
        {
            Debug.LogError("Pot GameObject not found in the scene.");
        }
    }


    public void Cook()
    {
        if (EvaluateResultForStrawberryJam() == true)
        {
            chefImage.enabled = true;
            chefImage.sprite = chefHappy;
            Debug.Log("The cooking was successful. Showing happy chef.");
        }
        else
        {
            chefImage.enabled = true;
            chefImage.sprite = chefSad;
            Debug.Log("The cooking was unsuccessful. Showing sad chef.");
        }

    }

    // Method to evaluate the result based on the pot's weight
    //Straberrirs= 900 gramm, sugar= 300 gramm
    public bool EvaluateResultForStrawberryJam()
    {
        Dictionary<string, float> ingredientWeights = potScript.CalculateWeightOfEachIngredient();

        // Check if the dictionary contains the required ingredients
        if (ingredientWeights.ContainsKey("Strawberry") && ingredientWeights.ContainsKey("Sugar"))
        {
            float strawberryWeight = ingredientWeights["Strawberry"];
            float sugarWeight = ingredientWeights["Sugar"];

            // Define the target weights
            float targetStrawberryWeight = 900f;
            float targetSugarWeight = 300f;

            // Define the acceptable tolerance
            float tolerance = 20f;

            // Check if both ingredients are within the acceptable range
            bool isStrawberryWithinRange = Mathf.Abs(strawberryWeight - targetStrawberryWeight) <= tolerance;
            bool isSugarWithinRange = Mathf.Abs(sugarWeight - targetSugarWeight) <= tolerance;

            // Log the results for debugging
            Debug.Log("Strawberry weight: " + strawberryWeight + " (Target: " + targetStrawberryWeight + " ? " + tolerance + ")");
            Debug.Log("Sugar weight: " + sugarWeight + " (Target: " + targetSugarWeight + " ? " + tolerance + ")");

            // Return true if both ingredients are within the acceptable range
            return isStrawberryWithinRange && isSugarWithinRange;
        }
        else
        {
            Debug.LogError("Required ingredients (Strawberry and Sugar) are missing.");
            return false;
        }
    }
    
}
