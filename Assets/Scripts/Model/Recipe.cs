using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Ingredient/Recipe")]
public class Recipe : ScriptableObject, IIngredient
{
    [SerializeField] private string ingredientName;
    [SerializeField] private float ingredientWeight;
    [SerializeField] private List<IngredientAmount> ingredients;
    [SerializeField] private string recipeInstruction;
    [SerializeField] private Sprite sprite;


    public string Name => ingredientName;
    public float Weight => ingredientWeight;
    public string Instruction => recipeInstruction;
    public Sprite Sprite => sprite;
    public List<IngredientAmount> GetIngredients => ingredients;
}
