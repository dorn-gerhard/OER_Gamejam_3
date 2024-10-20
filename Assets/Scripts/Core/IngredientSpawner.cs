using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] GameObject ingredientPrefab;
    [SerializeField] Recipe recipe;

    Vector3 start = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        start.x -= 2.5f;
        start.y -= 2;
        start.z -= 0.5f;
        SpawnRecipeIngredients();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && recipe != null)
        {
            //SpawnRecipeIngredients();
        }
    }

    public void SpawnRecipeIngredients()
    {
        foreach (IngredientAmount ingredientAmount in recipe.GetIngredients)
        {
            
            if (ingredientAmount.ingredient is BasicIngredient)
                SpawnObject(ingredientAmount.ingredient as BasicIngredient, null);
        }
    }

    public void SpawnObject(BasicIngredient ingredient, Transform transform)
    {
        GameObject newInstance = Instantiate(ingredientPrefab);
        newInstance.name = ingredient.Name;
        
        start.x -= 2f;
        newInstance.transform.position = transform != null ? transform.position : start;
        newInstance.transform.localScale *= 2;
        SpriteRenderer spriteRenderer = newInstance.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = ingredient.IngredientContainerSprite;

        BoxCollider2D boxCollider = newInstance.GetComponent<BoxCollider2D>();
        boxCollider.size = spriteRenderer.sprite.bounds.size;

        IngredientDataHolder dataHolder = newInstance.GetComponent<IngredientDataHolder>();
        dataHolder.ingredient = ingredient;

        Debug.Log($"{ingredient.Name} spawned.");

    }

  
}
