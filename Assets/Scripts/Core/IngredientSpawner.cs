using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] GameObject ingredientPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnObject(BasicIngredient ingredient, Transform transform)
    {
        GameObject newInstance = Instantiate(ingredientPrefab);
        newInstance.name = ingredient.Name;

        //newInstance.transform.position = Vector3.zero;
        newInstance.transform.position = transform != null ? transform.position : Vector3.zero;

        Debug.Log($"{ingredient.Name} spawned.");

        newInstance.GetComponent<Image>().sprite = ingredient.Sprite;
    }

  
}
