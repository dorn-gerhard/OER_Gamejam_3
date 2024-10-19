using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeSpawner : MonoBehaviour
{
    [SerializeField] private ScriptableObject recipe;
    // Start is called before the first frame update
    void Start()
    {
        foreach(IIngredient ingredient in (recipe as Recipe).GetIngredients)
        {
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
