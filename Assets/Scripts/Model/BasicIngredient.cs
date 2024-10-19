using UnityEngine;

[CreateAssetMenu(fileName = "NewBasicIngredient", menuName = "Ingredient/BasicIngredient")]
public class BasicIngredient : ScriptableObject, IIngredient
{
    [SerializeField] private string ingredientName;
    [SerializeField] private float ingredientWeight;
    [SerializeField] private Sprite sprite;

    public string Name => ingredientName;
    public float Weight => ingredientWeight;
    public Sprite Sprite => sprite;

}
