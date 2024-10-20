using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;
using TMPro;
using TexDrawLib;
using Platformer.Mechanics;

public class CutFunction : MonoBehaviour
{
    public LineRenderer lineRenderer;
    EdgeCollider2D edge;

    public FunctionType currentWeaponType = 0;

    public int numberOfPoints = 600;
    public float par1 = 0.5f;
    public float par2 = 1f;
    public bool drawNow = false;
    public bool fireNow = false;
    public List<Vector2> cutPoints;
    public Transform circle; 

    public float ComboConfidenceGain = 3;
    public Polygon brett;

    [Header("Slider")]
    
    [Header("TextInput")]
  
    //public float RandomizedValue;

    float angle = 0;

    [Serializable]
    public struct LineLengthData
    {
        public float startValue;
        public float endValue;
        public float maxLength;
    }

    LineLengthData currentLineLengthData;

    // Start is called before the first frame update
    void Start()
    {
        edge = GetComponent<EdgeCollider2D>();

        currentLineLengthData.startValue = -6;
        currentLineLengthData.endValue = 6;
        currentLineLengthData.maxLength =  200;

    }
    private void OnValidate()
    {
        if (drawNow)
        {
            Draw();
            Debug.Log("Draw Now");

            drawNow = false;
        }

        if (fireNow)
        {
            StartCoroutine(Cut());
            Debug.Log("Fire Now");
            fireNow = false;
        }


    }

    // Update is called once per frame
    void Update()
    {

        Draw();
    }

    public void SetLineLength(LineLengthData newLineLengthData)
    {
        currentLineLengthData = newLineLengthData;
    }

    public IEnumerator Cut()
    {

        // Animation
        cutPoints = edge.points.ToList<Vector2>();
        yield return StartCoroutine(LaserBeam(2.0f));

        CutCurveWithPolygon();

        yield return null;
    }

    public Polygon CutCurveWithPolygon()
    {
        /*
        int oldLaserPos = 0;
        // -1 below lower, 0 in between, 1 above upper
        int laserPos = 0;
        float xValue = 0;
        float laserValue = 0;
        Vector2 closestPointUpper = new Vector2();
        for (int k = 0; k < brett.nPoints; k++)
        {
            xValue = brett.lowerPoints[k].x;

            laserValue = cutPoints[cutPoints.Select((x, index) => (Math.Abs(x.x - xValue), index)).Min().index].y;

            if (laserValue < brett.lowerPoints[k].y)
            {

                laserPos = -1;
                if (oldLaserPos == -1)
                {
                    // laser remains below - lower and upper limit remain the same
                }
                else if (oldLaserPos == 0)
                {
                    // laser enters area -> split in two polygons
                    brett.lowerPoints[k].y = laserValue;

                    // create new Polygon
                    // min = xValue
                    // max ... unknown
                    // new.lowerPoints[0] = brett.lowerPoint[k]
                }



            }
            else if (laserValue < brett.upperPoints[k].y)
            {
                laserPos = 0;
            }
            else
            {
                laserPos = 1;
            }

            
        }
        */
        //brett.points = 
        return brett;
    }


    AnimationCurve GetLaserCurve(float t, float width)
    {
        float height = 0.3f;
        if (t < width / 2)
            return new AnimationCurve(new Keyframe(0, 0),
                                      new Keyframe(0.5f - t - width / 2, 0.05f),
                                      new Keyframe(0.5f - t, height),
                                      new Keyframe(0.5f, height * (width / 2 - t) / width / 2),
                                      new Keyframe(0.5f + t, height),
                                      new Keyframe(0.5f + t + width / 2, 0.05f),
                                      new Keyframe(1, 0));
        else
            return new AnimationCurve(new Keyframe(0, 0),
                                      new Keyframe(0.5f - t - width / 2, 0.05f),
                                      new Keyframe(0.5f - t, height),
                                      new Keyframe(0.5f - t + width / 2, 0.05f),
                                      new Keyframe(0.5f + t - width / 2, 0.05f),
                                      new Keyframe(0.5f + t, height),
                                      new Keyframe(0.5f + t + width / 2, 0.05f),
                                      new Keyframe(1, 0));
    }

    public IEnumerator LaserBeam(float waitTime)
    {
        int k = 0;
        int k_final = 400;
        while (k < k_final)
        {
            yield return new WaitForSecondsRealtime(0.004f);
            //print("WaitAndPrint " + Time.unscaledTime);
            lineRenderer.widthCurve = GetLaserCurve((float)k / (2.0f + (float)k_final), 0.2f);
            k++;
        }
    }

    private Vector3[] GetPath(float startValue, float endValue, float maxLengthOverride)
    {
        float increment = (endValue - startValue) / (numberOfPoints + 1);
        float[] parameterVals = new float[numberOfPoints];
        float[] functionVals = new float[numberOfPoints];
        Vector3[] vertexPositions = new Vector3[numberOfPoints];
        float length = 0.0f;
        int lengthOfVector = numberOfPoints;
        for (int i = 0; i < numberOfPoints; i++)
        {
            float yValue = 0;
            if (currentWeaponType == FunctionType.linear)
            {
                yValue = EvalFunctionLinear(startValue + i * increment, par1, par2);
            }
            else if (currentWeaponType == FunctionType.quadratic)
            {
                yValue = EvalFunctionQuadratic(startValue + i * increment, par1, par2);
            }
            else if (currentWeaponType == FunctionType.sin)
            {
                yValue = EvalFunctionSin(startValue + i * increment, par1, par2);
            }

            Vector3 temp_vector = new Vector3(startValue + i * increment, yValue, 0);
            vertexPositions[i] = Quaternion.AngleAxis(angle, Vector3.forward) * temp_vector + transform.parent.transform.position;

            if (i == 0) continue;

            length += (vertexPositions[i] - vertexPositions[i - 1]).magnitude;
            if (length > maxLengthOverride)
            {
                lengthOfVector = i;
                break;
            }
        }

        // shorten vector
        Array.Resize(ref vertexPositions, lengthOfVector);
        return vertexPositions;
    }

    public LayerMask layerMask;   // The layer(s) to check for collisions

    public Vector3[] Draw()
    {
        Vector3[] vertexPositions = CalculateVertexPositions();
        Vector3[] shiftedVertexPositions = ShiftVertexPositions(vertexPositions);

        DrawLine(vertexPositions.Length, vertexPositions);
        lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, 0.1f), new Keyframe(1, 0.1f));
        addCollider(shiftedVertexPositions.ToList());

        

        return vertexPositions;
    }

    float GetDistanceToEdge(Collider2D collider, EdgeCollider2D edge)
    {
        if (collider == null) return Mathf.Infinity;

        // Find the closest point on the edgeCollider and calculate the distance
        Vector2 closestPoint = edge.ClosestPoint(collider.transform.position);
        return Vector2.Distance(collider.transform.position, closestPoint);
    }

    public Vector3[] CalculateVertexPositions()
    {
        Vector3[] vertexPositionsPositive = GetPath(0.0f, currentLineLengthData.endValue, currentLineLengthData.maxLength);
        Vector3[] vertexPositionsNegative = GetPath(0.0f, currentLineLengthData.startValue, currentLineLengthData.maxLength);

        Array.Reverse(vertexPositionsNegative);

        Vector3[] vertexPositions = new Vector3[vertexPositionsNegative.Length + vertexPositionsPositive.Length];

        vertexPositionsNegative.CopyTo(vertexPositions, 0);
        vertexPositionsPositive.CopyTo(vertexPositions, vertexPositionsNegative.Length);

        return vertexPositions;
    }

    public Vector3[] ShiftVertexPositions(Vector3[] vertexPositions)
    {
        Vector3[] shiftedVertexPositions = new Vector3[vertexPositions.Length];

        // add shift to real coordinates
        for (int i = 0; i < vertexPositions.Length; i++)
        {
            shiftedVertexPositions[i] = vertexPositions[i] - transform.parent.transform.position;
        }

        return shiftedVertexPositions;
    }

    void addCollider(List<Vector3> colliderPoints)
    {
        List<Vector2> colliderPoints2 = new List<Vector2>();

        for (int i = 0; i < colliderPoints.Count; i++)
        {
            colliderPoints2.Add(new Vector3(colliderPoints[i].x, colliderPoints[i].y, 0f));
        }

        //for (int i = colliderPoints.Count - 1; i > 0; i--)
        //{
        //    colliderPoints2.Add(new Vector3(colliderPoints[i].x, colliderPoints[i].y, 0f));
        //}

        edge.points = colliderPoints2.ToArray();
    }

    void DrawLine(int nPoints, Vector3[] vertexPositions)
    {
        lineRenderer.positionCount = nPoints;

        //lineRenderer.colorGradient = PlayerController.current.GetCurrentFunctionScriptableObject().gradient;

        lineRenderer.SetPositions(vertexPositions);
    }

    float EvalFunctionLinear(float x, float param1, float param2)
    {
        return param1 * x + param2;
    }

    float EvalFunctionQuadratic(float x, float param1, float param2)
    {
        return param1 * x * x + param2;
    }

    float EvalFunctionSin(float x, float param1, float param2)
    {
        return param2 * (float)Math.Sin(param1 * x);
    }


    public void UpdatePar1(float newPar1)
    {
        par1 = newPar1;
    }

    public void UpdatePar2(float newPar2)
    {
        par2 = newPar2;
    }

   

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Name of collider: " + collision.name);
        if (collision.gameObject.GetComponent<EnemyController>())
        {
            collision.gameObject.GetComponent<EnemyController>().health.Decrement(PlayerController.current.GetCurrentFunctionScriptableObject().type);
        }
    }

    [Serializable]
    public struct DynamicFunctionParts
    {
        public string text1;
        public string text2;
        public string text3;

        public string input1;
        public string input2;
    }



    public string GetFunctionString()
    {
        return GetFunctionString(currentWeaponType);
    }

    public string GetFunctionString(FunctionType functionType)
    {
        string functionString = "";
        if (functionType == FunctionType.linear)
        {
            functionString = "k * x + d";
        }
        else if (functionType == FunctionType.quadratic)
        {
            functionString = "a * x^2 + b";
        }
        else if (functionType == FunctionType.sin)
        {
            functionString = "a * sin(b * x)";
        }
        return functionString;
    }
}
