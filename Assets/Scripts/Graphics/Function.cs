using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Function : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Slider rotationSlider;
    public Slider parameter1;
    public Slider parameter2;

    // Start is called before the first frame update
    void Start()
    {
        rotationSlider.minValue = -180;
        rotationSlider.maxValue = 180;

        parameter1.minValue = -10;
        parameter2.minValue = -10;
        parameter1.maxValue = 10;
        parameter2.maxValue = 10;

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Draw()
    {
        Debug.Log("Parameter: " + rotationSlider.value);
        int numberofpoints = 200;
        float startValue = -10.0f;
        float endValue = 10.0f;
        float increment = (endValue - startValue) / (numberofpoints+1);
        float[] parameterVals = new float[numberofpoints];
        float[] functionVals = new float[numberofpoints];
        Vector3[] vertexPositions = new Vector3[numberofpoints];

        for (int i = 0; i < numberofpoints; i++)
        {

            float yValue = EvalFunction(startValue + i * increment, parameter1.value, parameter2.value);
            Vector3 temp_vector = new Vector3(startValue + i * increment, yValue, 0);
            vertexPositions[i] = Quaternion.AngleAxis(rotationSlider.value, Vector3.up) * temp_vector; 
        }
        DrawLine(numberofpoints, vertexPositions);

    }
    void DrawLine(int nPoints, Vector3[] vertexPositions)
    {

        lineRenderer.positionCount = nPoints;
        lineRenderer.SetPositions(vertexPositions);
    }

    float EvalFunction(float x, float param1, float param2)
    {
        return param2 * (float)Math.Sin(param1 * x);
    }
}
