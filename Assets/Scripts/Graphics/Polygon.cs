using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Polygon : MonoBehaviour
{
    // Start is called before the first frame update
    
    public float[] xgrid;
    public float[] lower;
    public float[] upper;
    
    public PolygonCollider2D polygon;

    public bool setPolygon = false;
    public bool initBoard = false;
    public bool comparePolygon = false;

    public void OnValidate()
    {

       
        if (setPolygon)
        {
            SetPolygon();
            setPolygon = false;
        }

        if (comparePolygon)
        {
            ComparePolygon();
            comparePolygon = false;
        }
        if (initBoard)
        {
            InitBoard(-3, 3, 500, 0, 8);
            SetPolygon();
            initBoard = false;

        }
    }

    private void Start()
    {
        polygon = GetComponent<PolygonCollider2D>();
        
    }
    public void SetPolygon()
    {
        Vector2[] vertexPositions = new Vector2[xgrid.Length * 2];

        for (int k = 0; k < xgrid.Length; k++)
        {
            vertexPositions[k] = new Vector2(xgrid[k], upper[k]);
            vertexPositions[xgrid.Length + k] = new Vector2(xgrid[xgrid.Length - 1 - k], lower[xgrid.Length - 1 - k]);
        }

        //Vector2[] flippedLowerPoints = new Vector2[lowerPoints.Length];
        //Array.Copy(lowerPoints, flippedLowerPoints, lowerPoints.Length);
        //Array.Reverse(flippedLowerPoints);

        //flippedLowerPoints.CopyTo(vertexPositions, 0);
        //upperPoints.CopyTo(vertexPositions, lowerPoints.Length);
        polygon.points = vertexPositions;
    }


    public void InitBoard(float minXInput, float maxXInput, int numberOfGridPoints, float minYInput, float maxYInput)
    {
        
       
        xgrid = new float[numberOfGridPoints];
        lower = new float[numberOfGridPoints];
        upper = new float[numberOfGridPoints];
        float increment = (maxXInput - minXInput) / numberOfGridPoints;

        for (int k = 0; k < numberOfGridPoints; k++)
        {
            xgrid[k] = minXInput + k * increment;
            lower[k] = minYInput;
            upper[k] = maxYInput;
           
        }

    }
    public void UpdatePolygon(float minXInput, float maxXInput, float minYInput, float maxYInput, float[] xgridInput, float[] lowerPointsInput, float[] upperPointsInput, int numberPoints, PolygonCollider2D polygonInput)
    {
        
        
        xgrid = xgridInput;
        lower = lowerPointsInput;
        upper = upperPointsInput;
        polygon = polygonInput;
        SetPolygon();

    }

    public float ComparePolygon()
    {
        Polygon reference = GameObject.FindGameObjectWithTag("Ziel").GetComponent<Polygon>();

        float gridMinX = Math.Min(xgrid.Min(), reference.xgrid.Min());
        float gridMaxX = Math.Max(xgrid.Max(), reference.xgrid.Max());
        float gridMinY = Math.Min(lower.Min(), reference.lower.Min());
        float gridMaxY = Math.Max(upper.Max(), reference.upper.Max());

        float xrange = gridMaxX - gridMinX;
        float yrange = gridMaxY - gridMinY;

        float increment = Math.Max(xrange, yrange) / 999;

        int ixMax = (int)Math.Ceiling(xrange / increment);
        int iyMax = (int)Math.Ceiling(yrange / increment);

        GameObject  cursor= new GameObject();
        //Instantiate(cursor);
        
    
        

        BoxCollider2D box = cursor.AddComponent<BoxCollider2D>();

        box.size = new Vector2(increment, increment);
        ContactFilter2D mode = new ContactFilter2D();
        mode = mode.NoFilter();

        List<Collider2D> collisions = new List<Collider2D>();
        int union = 0;
        int intersection = 0;

        for (int ix = 0; ix < ixMax; ix++)
        {
            for (int iy = 0; iy < iyMax; iy++)
            {
                box.transform.position = new Vector3(gridMinX + ix * increment, gridMinY + iy * increment, 0);

                
                box.OverlapCollider(mode, collisions);
                if (collisions.Contains(polygon) || collisions.Contains(reference.polygon))
                {
                    union += 1;
                }

                if (collisions.Contains(polygon) && collisions.Contains(reference.polygon))
                {
                    intersection += 1;
                }



            }
        }

        Debug.Log("Overlap: " + intersection / union + " %");

        return intersection / union;

    }


}
