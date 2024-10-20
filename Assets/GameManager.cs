using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Pot potScript; // Reference to the Pot script

    // Start is called before the first frame update
    void Start()
    {
        // Find the Pot GameObject in the scene
        GameObject potObject = GameObject.Find("Pot"); // Make sure the Pot GameObject is named "Pot" in the scene
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
            //load the picture with happy chef
        }
        else
        {
            //load the picture with sad chef
        }

    }

    // Method to evaluate the result based on the pot's weight
    //Straberrirs= 500 gramm, sugar= 375 gramm
    public bool EvaluateResultForStrawberryJam()
    {
        Dictionary<string, float> ingredientWeights = potScript.CalculateWeightOfEachIngredient();

        // Check if the dictionary contains the required ingredients
        if (ingredientWeights.ContainsKey("Strawberry") && ingredientWeights.ContainsKey("Sugar"))
        {
            float strawberryWeight = ingredientWeights["Strawberry"];
            float sugarWeight = ingredientWeights["Sugar"];

            // Define the target weights
            float targetStrawberryWeight = 500f;
            float targetSugarWeight = 375f;

            // Define the acceptable tolerance
            float tolerance = 20f;

            // Check if both ingredients are within the acceptable range
            bool isStrawberryWithinRange = Mathf.Abs(strawberryWeight - targetStrawberryWeight) <= tolerance;
            bool isSugarWithinRange = Mathf.Abs(sugarWeight - targetSugarWeight) <= tolerance;

            // Log the results for debugging
            Debug.Log("Strawberry weight: " + strawberryWeight + " (Target: " + targetStrawberryWeight + " ± " + tolerance + ")");
            Debug.Log("Sugar weight: " + sugarWeight + " (Target: " + targetSugarWeight + " ± " + tolerance + ")");

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
