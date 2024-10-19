using UnityEngine;

public class PositionUIImage : MonoBehaviour
{
    public GameObject targetGameObject;  // The gameobject in 3D space
    public RectTransform uiElement;      // The UI element to be positioned
    public Canvas canvas;                // The Canvas the UI element is on
    public Vector2 offset;               // Offset for positioning under the object

    void Update()
    {
        // Get the position of the GameObject in the world space
        Vector3 worldPos = targetGameObject.transform.position;

        // Convert world position to screen space
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // Convert screen position to canvas space
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out canvasPos);

        // Apply the offset (to move the image under the GameObject)
        canvasPos += (targetGameObject.transform.position.y > -13) ? offset : -offset;

        // Set the position of the UI element
        uiElement.anchoredPosition = canvasPos;
    }
}
