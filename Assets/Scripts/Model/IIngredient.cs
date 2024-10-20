using UnityEngine;

public interface IIngredient
{
    string Name {get;}
    float Weight {get;}
    Sprite IngredienContainerSprite { get; }
    Sprite IngredientPortionSprite { get; }
}
