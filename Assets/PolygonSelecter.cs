using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonSelecter : MonoBehaviour
{
    public float selectionLineWidth = 0.4f;

    FillPolygon fillPolygon;
    LineRenderer lineRenderer;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        SetSelected(false);
    }

    void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            lineRenderer.startWidth = selectionLineWidth;
            lineRenderer.endWidth = selectionLineWidth;
        }
        else
        {
            lineRenderer.startWidth = 0;
            lineRenderer.endWidth = 0;
        }
    }

    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Mouse is over GameObject.");

        SetSelected(true);
    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Mouse is no longer on GameObject.");

        SetSelected(false);
    }

    private void OnMouseUpAsButton()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Selected GameObject." + gameObject.name);

        TischlerGameController.current.SelectPolygon(gameObject);
    }
}
