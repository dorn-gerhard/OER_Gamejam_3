using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnPortion : MonoBehaviour
{
    [SerializeField] GameObject prefabOfNewPortionIngredient;

    private Vector3 offset;
    private Camera mainCamera;
    private bool isDragging = false;
    private GameObject newInstance;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        offset = transform.position - mousePosition;

        IngredientDataHolder data= gameObject.GetComponent<IngredientDataHolder>();
        isDragging = true;
        SpawnNewPortion(data.ingredient as BasicIngredient, mousePosition + offset);


    }

    public void SpawnNewPortion(BasicIngredient ingredient, Vector3 position)
    {
        newInstance = Instantiate(prefabOfNewPortionIngredient);
        newInstance.name = ingredient.Name;

        IngredientDataHolder portionData = newInstance.GetComponent<IngredientDataHolder>();
        portionData.ingredient = ingredient;

        //newInstance.transform.position = Vector3.zero;
        newInstance.transform.position = position != null ? position : Vector3.zero;
        newInstance.transform.localScale *= 0.75f;
        SpriteRenderer spriteRenderer = newInstance.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = ingredient.IngredientPortionSprite;

        BoxCollider2D boxCollider = newInstance.GetComponent<BoxCollider2D>();
        boxCollider.size = spriteRenderer.sprite.bounds.size;


        //portionData.isDragging = true;
        //portionData.offset = newInstance.transform.position - GetMouseWorldPosition();
        Debug.Log($"{ingredient.Name} spawned.");
    }

    void OnMouseDrag()
    {
        if (isDragging && newInstance != null)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            newInstance.transform.position = mousePosition + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        newInstance.GetComponent<IngredientDataHolder>().isDraggable = true;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = 0f; // For 2D, z is irrelevant, 
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
}
