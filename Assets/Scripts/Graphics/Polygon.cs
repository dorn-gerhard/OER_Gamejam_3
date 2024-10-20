using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon : MonoBehaviour
{
    // Start is called before the first frame update
    public float minX;
    public float maxX;
    public Vector2[] lowerPoints;
    public Vector2[] upperPoints;
    public int nPoints;
    public float minY;
    public float maxY;
    PolygonCollider2D polygon;

    public bool setPolygon = false;
    public bool initBoard = false;

    private void Start()
    {
        polygon = GetComponent<PolygonCollider2D>();
        InitBoard(-3, 3, 500, 0, 3);
        SetPolygon();
    }
    public void SetPolygon()
    {
        Vector2[] vertexPositions = new Vector2[lowerPoints.Length + upperPoints.Length];

        Vector2[] flippedLowerPoints = new Vector2[lowerPoints.Length];
        Array.Copy(lowerPoints, flippedLowerPoints, lowerPoints.Length);
        Array.Reverse(flippedLowerPoints);

        flippedLowerPoints.CopyTo(vertexPositions, 0);
        upperPoints.CopyTo(vertexPositions, lowerPoints.Length);
        polygon.points = vertexPositions;
    }

    private void OnValidate()
    {
        if (setPolygon)
        {
            SetPolygon();
            setPolygon = false;
        }

        if (initBoard)
        {
            InitBoard(-3, 3, 500, 0, 3);
            initBoard = false;
        }
    }

    public void InitBoard(float minXInput, float maxXInput, int numberOfGridPoints, float minYInput, float maxYInput)
    {
        minX = minXInput;
        maxX = maxXInput;
        minY = minYInput;
        maxY = maxYInput;
        nPoints = numberOfGridPoints;
        lowerPoints = new Vector2[numberOfGridPoints];
        upperPoints = new Vector2[numberOfGridPoints];
        float increment = (maxX - minX) / nPoints;

        for (int k = 0; k < numberOfGridPoints; k++)
        {
            lowerPoints[k] = new Vector2(minX + k * increment, minYInput);
            upperPoints[k] = new Vector2(minX + k * increment, maxYInput);
        }

    }

   

}
