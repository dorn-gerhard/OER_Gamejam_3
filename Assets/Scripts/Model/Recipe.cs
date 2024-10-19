using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Ingredient/Recipe")]
public class Recipe : ScriptableObject, IIngredient
{
    [SerializeField] private string ingredientName;
    [SerializeField] private float ingredientWeight;
    [SerializeField] private List<IngredientAmount> ingredients;
    [SerializeField] private string recipeInstruction;


    public string Name => ingredientName;
    public float Weight => ingredientWeight;
    public string Instruction => recipeInstruction;
}
