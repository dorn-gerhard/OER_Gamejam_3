using UnityEngine;

[CreateAssetMenu(fileName = "NewBasicIngredient", menuName = "Ingredient/BasicIngredient")]
public class BasicIngredient : ScriptableObject, IIngredient
{
    [SerializeField] private string ingredientName;
    [SerializeField] private float ingredientWeight;

    public string Name => ingredientName;
    public float Weight => ingredientWeight;
}
