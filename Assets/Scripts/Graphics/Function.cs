using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;

public class Function : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Slider rotationSlider;
    public Slider parameter1;
    public Slider parameter2;
    public Projectile projectile;

    public int numberOfPoints = 200;
    public float par1 = 0.5f;
    public float par2 = 1f;
    float startValue = -2.0f;
    float endValue = 2.0f;
    float angle = 0;

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
        Draw();
        if (Input.GetMouseButtonDown(0))
        {
            SpawnProjectile();
        }
        //angle = Math.Clamp(angle + Input.GetAxis("Mouse ScrollWheel") * 180.0f, -180.0f, 180.0f);
        par1 = Math.Clamp(par1 + Input.GetAxis("Mouse ScrollWheel") * 5, -5, 5);
    }

    public void SpawnProjectile()
    {
        List<Vector3> vec = GetPath(0.0f, endValue).ToList();
        Instantiate(projectile).InitializePath(vec);

        List<Vector3> vec2 = GetPath(0.0f, startValue).ToList();
        Instantiate(projectile).InitializePath(vec2);
    }

    private Vector3[] GetPath(float startValue, float endValue)
    {        
        float increment = (endValue - startValue) / (numberOfPoints + 1);
        float[] parameterVals = new float[numberOfPoints];
        float[] functionVals = new float[numberOfPoints];
        Vector3[] vertexPositions = new Vector3[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {

            float yValue = EvalFunction(startValue + i * increment, par1, par2);
            Vector3 temp_vector = new Vector3(startValue + i * increment, yValue, 0);
            vertexPositions[i] = Quaternion.AngleAxis(angle, Vector3.forward) * temp_vector + transform.parent.transform.position;
        }
        return vertexPositions;
    }

    public void Draw()
    {
        Vector3[] vertexPositions = GetPath(startValue, endValue);
        DrawLine(numberOfPoints, vertexPositions);

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

    public void UpdateAngle()
    {
        angle = rotationSlider.value;
    }

    public void UpdatePar1()
    {
        par1 = parameter1.value;
    }

    public void UpdatePar2()
    {
        par2 = parameter2.value;
    }
}
