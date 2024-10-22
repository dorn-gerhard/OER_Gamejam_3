using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] GameObject ingredientPrefab;
    [SerializeField] Recipe recipe;

    private List<Transform> freeSpawnSlots;

    // Start is called before the first frame update
    void Start()
    {
        freeSpawnSlots = transform.Cast<Transform>().Where(child => child.name.Contains("IngredientSlot")).ToList();
        
        SpawnRecipeIngredients();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnRecipeIngredients()
    {
        foreach (IngredientAmount ingredientAmount in recipe.GetIngredients)
        {
            
            if (ingredientAmount.ingredient is BasicIngredient)
                SpawnObject(ingredientAmount.ingredient as BasicIngredient);
        }
    }

    public void SpawnObject(BasicIngredient ingredient)
    {
        Transform spawnSlot = popFreeSlot();

        GameObject newInstance = Instantiate(ingredientPrefab, spawnSlot);

        newInstance.name = ingredient.Name;
        newInstance.transform.position = spawnSlot.position;

        SpriteRenderer spriteRenderer = newInstance.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = ingredient.IngredientContainerSprite;

        BoxCollider2D boxCollider = newInstance.GetComponent<BoxCollider2D>();
        boxCollider.size = spriteRenderer.sprite.bounds.size;

        IngredientDataHolder dataHolder = newInstance.GetComponent<IngredientDataHolder>();
        dataHolder.ingredient = ingredient;

        Debug.Log($"{ingredient.Name} Container spawned.");

    }

    private Transform popFreeSlot()
    {
        Transform lastTransform = freeSpawnSlots[freeSpawnSlots.Count - 1];
        freeSpawnSlots.RemoveAt(freeSpawnSlots.Count - 1);
        return lastTransform;
    }

}
