using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    IngredientDataHolder data;
    private bool isDragging = false;

    void Start()
    {
        mainCamera = Camera.main;
        data = gameObject.GetComponent<IngredientDataHolder>();
    }

    void OnMouseDown()
    {
        if (data != null)
        {
            if (data.isDraggable)
            {
                Vector3 mousePosition = GetMouseWorldPosition();
                offset = transform.position - mousePosition;
                isDragging = true;
            }
        }
    }

    void OnMouseDrag()
    {
        if (isDragging && data.isDraggable)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            transform.position = mousePosition + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = 0f; // For 2D, z is irrelevant, 
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
}
