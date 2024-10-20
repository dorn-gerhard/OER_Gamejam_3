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
    public LineRenderer lineRenderer; // needs global coordinates
    EdgeCollider2D edge; // needs local coordinates

    public FunctionType currentWeaponType = 0;

    public int numberOfPoints = 600;
    public float par1 = 0.5f;
    public float par2 = 1f;
    public bool drawNow = false;
    public bool fireNow = false;
    public List<Vector2> cutPoints;
    public Transform circle; 

    public float ComboConfidenceGain = 3;

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
        Vector3 shift = transform.parent.transform.position;
        cutPoints = edge.points.ToList<Vector2>().Select(x => new Vector2(x.x + shift.x, x.y  + shift.y)).ToList();
        yield return StartCoroutine(LaserBeam(2.0f)); 
        GameObject brett2 = FindObjectOfType<PolygonSelecter>().gameObject;
        CutCurveWithPolygon(brett2);

        yield return null;
    }

    public Polygon CutCurveWithPolygon(GameObject brett)
    {
        Polygon brettPolygon = brett.GetComponent<Polygon>();
        
        int oldLaserPos = 0;
        // -1 below lower, 0 in between, 1 above upper
        int laserPos = 0;
        float xValue = 0;
        float laserValue = 0;
        float lowerValue = 0;
        float upperValue = 0;

        Vector2 closestPointUpper = new Vector2();
        int numPoints = brettPolygon.xgrid.Length;
        float[] lower = new float[numPoints];
        float[] middle = new float[numPoints];
        float[] upper = new float[numPoints];
        int[] laser = new int[numPoints];
        bool[] upValid = new bool[numPoints];
        bool[] lowValid = new bool[numPoints];

        int startUpper = 0;
        int endUpper = 0;
        int startLower = 0;
        int endLower = 0;

        Debug.Log("Number of grid points: " + (brettPolygon.xgrid.Length));
        for (int k = 0; k < brettPolygon.xgrid.Length; k++)
        {
            xValue = brettPolygon.xgrid[k];

            laserValue = cutPoints[cutPoints.Select((x, index) => (Math.Abs(x.x - xValue), index)).Min().index].y;
            lowerValue = brettPolygon.lower[k];
            upperValue = brettPolygon.upper[k];

            if (laserValue < lowerValue) // laser is below board
            {
                lower[k] = laserValue;
                middle[k] = lowerValue;
                upper[k] = upperValue;

                lowValid[k] = false;
                upValid[k] = true;
                laser[k] = -1;

                if ((k > 0) && lowValid[k - 1])
                {
                    // laser left lower part
                    // create Polygon starting from startLower till endLower
                    GameObject newBrett = Instantiate(brett);
                    newBrett.name = "newBrettLower";
                    Polygon newPolygon = newBrett.GetComponent<Polygon>();
                    PolygonCollider2D polygonCollider = newBrett.GetComponent<PolygonCollider2D>();
                    newPolygon.UpdatePolygon(brettPolygon.xgrid[startLower], brettPolygon.xgrid[k], lower[startLower..k].Min(), middle[startLower..k].Max(), brettPolygon.xgrid[startLower..k], lower[startLower..k], middle[startLower..k], k - startLower, polygonCollider);



                }
            }
            else
            {
                lower[k] = lowerValue;
                if (laserValue > upperValue)  // laser is above board
                {
                    upper[k] = laserValue;
                    middle[k] = upperValue;

                    laser[k] = 1;
                    lowValid[k] = true;
                    upValid[k] = false;

                    if ((k > 0) && upValid[k - 1])
                    {
                        // laser just has left upper part
                        // Create Polygon starting from startUpper till now
                        GameObject newBrett = Instantiate(brett);

                        newBrett.name = "newBrettUpper";
                        Polygon newPolygon = newBrett.GetComponent<Polygon>();
                        PolygonCollider2D polygonCollider = newBrett.GetComponent<PolygonCollider2D>();
                        newPolygon.UpdatePolygon(brettPolygon.xgrid[startUpper],
                            brettPolygon.xgrid[k],
                            lower[startUpper..k].Min(),
                            middle[startUpper..k].Max(),
                            brettPolygon.xgrid[startUpper..k],
                            middle[startUpper..k],
                            upper[startUpper..k],
                            k - startUpper,
                            polygonCollider);


                    }
                }
                else // laser is in between
                {
                    upper[k] = upperValue;
                    middle[k] = laserValue;

                    laser[k] = 0;
                    lowValid[k] = true;
                    upValid[k] = true;

                    if ((k > 0) && !upValid[k - 1])
                    {
                        // laser was up before -> set startUpper
                        startUpper = k;
                    }

                    if ((k > 0) && !lowValid[k - 1])
                    {
                        // laser enters from below -> set startLower
                        startLower = k;
                    }
                }
            }
        }

        endLower = laser.Length - 1;
        endUpper = laser.Length - 1;
        // handle right side handling
        // three posibilities:
        Debug.Log("laser length: " + laser.Length);
        Debug.Log("laser last: " + (laser.Last()));

        Debug.Log("startLower: " + startLower + ", start Upper: " + startUpper);
        if (laser.Last() == 1) // above
        {
            // laser is above
            // create Polygon starting from startLower till endLower
            GameObject newBrett = Instantiate(brett);
            newBrett.name = "newBrettLowerRightSide";
            Polygon newPolygon = newBrett.GetComponent<Polygon>();
            PolygonCollider2D polygonCollider = newBrett.GetComponent<PolygonCollider2D>();
           
            newPolygon.UpdatePolygon(
                brettPolygon.xgrid[startLower],
                brettPolygon.xgrid[endLower],
                lower[startLower..^0].Min(),
                middle[startLower..^0].Max(),
                brettPolygon.xgrid[startLower..^0],
                lower[startLower..^0],
                middle[startLower..^0],
                brettPolygon.xgrid.Length - startLower,
                polygonCollider);
        }
        else if (laser.Last() == -1) // below
        {
            // laser is below
            GameObject newBrett = Instantiate(brett);

            newBrett.name = "newBrettUpperRightSide";
            Polygon newPolygon = newBrett.GetComponent<Polygon>();
            PolygonCollider2D polygonCollider = newBrett.GetComponent<PolygonCollider2D>();
            newPolygon.UpdatePolygon(
                brettPolygon.xgrid[startUpper],
                brettPolygon.xgrid[endUpper],
                lower[startUpper..^0].Min(),
                middle[startUpper..^0].Max(),
                brettPolygon.xgrid[startUpper..^0],
                middle[startUpper..^0],
                upper[startUpper..^0],
                brettPolygon.xgrid.Length - startUpper,
                polygonCollider);
        }

        
        else
        {
            // laser is in between - create two parts

            // laser is above
            // create Polygon starting from startLower till endLower
            GameObject newBrett = Instantiate(brett);
            newBrett.name = "newBrettLowerRightSide";
            Polygon newPolygon = newBrett.GetComponent<Polygon>();
            PolygonCollider2D polygonCollider = newBrett.GetComponent<PolygonCollider2D>();
            newPolygon.UpdatePolygon(
                brettPolygon.xgrid[startLower],
                brettPolygon.xgrid[endLower],
                lower[startLower..^0].Min(),
                middle[startLower..^0].Max(),
                brettPolygon.xgrid[startLower..^0],
                lower[startLower..^0],
                middle[startLower..^0],
                brettPolygon.xgrid.Length - startLower,
                polygonCollider);


            // laser is below
            GameObject newBrett2 = Instantiate(brett);

            newBrett2.name = "newBrettUpperRightSide";
            Polygon newPolygon2 = newBrett2.GetComponent<Polygon>();
            PolygonCollider2D polygonCollider2 = newBrett2.GetComponent<PolygonCollider2D>();
                
            newPolygon2.UpdatePolygon(
                brettPolygon.xgrid[startUpper],
                brettPolygon.xgrid[endUpper],
                lower[startUpper..].Min(),
                middle[startUpper..^0].Max(),
                brettPolygon.xgrid[startUpper..^0],
                middle[startUpper..^0],
                upper[startUpper..^0],
                brettPolygon.xgrid.Length - startUpper,
                polygonCollider2);

        }
        


        Destroy(brett);





        
        
        //brett.points = 
        return brettPolygon;
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
            //yield return new WaitForSecondsRealtime(0.004f);
            //print("WaitAndPrint " + Time.unscaledTime);
            lineRenderer.widthCurve = GetLaserCurve((float)k / (2.0f + (float)k_final), 0.2f);
            k++;
        }
        yield return null;
    }

  

    public LayerMask layerMask;   // The layer(s) to check for collisions

    public Vector3[] Draw()
    {
        Vector3[] globalVertexPositions = CalculateVertexPositions();
        Vector3[] localVertexPositions = ShiftVertexPositions(globalVertexPositions);

        DrawLine(globalVertexPositions.Length, globalVertexPositions);
        lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, 0.1f), new Keyframe(1, 0.1f));
        addCollider(localVertexPositions.ToList());

        

        return globalVertexPositions;
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
        Vector3[] globalVertexPositionsPositive = GetPath(0.0f, currentLineLengthData.endValue, currentLineLengthData.maxLength);
        Vector3[] globalVertexPositionsNegative = GetPath(0.0f, currentLineLengthData.startValue, currentLineLengthData.maxLength);

        Array.Reverse(globalVertexPositionsNegative);

        Vector3[] globalVertexPositions = new Vector3[globalVertexPositionsNegative.Length + globalVertexPositionsPositive.Length];

        globalVertexPositionsNegative.CopyTo(globalVertexPositions, 0);
        globalVertexPositionsPositive.CopyTo(globalVertexPositions, globalVertexPositionsNegative.Length);

        return globalVertexPositions;
    }

    private Vector3[] GetPath(float startValue, float endValue, float maxLengthOverride)
    {
        float increment = (endValue - startValue) / (numberOfPoints + 1);
        float[] parameterVals = new float[numberOfPoints];
        float[] functionVals = new float[numberOfPoints];
        Vector3[] globalVertexPositions = new Vector3[numberOfPoints];
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
            globalVertexPositions[i] = Quaternion.AngleAxis(angle, Vector3.forward) * temp_vector + transform.parent.transform.position;

            if (i == 0) continue;

            length += (globalVertexPositions[i] - globalVertexPositions[i - 1]).magnitude;
            if (length > maxLengthOverride)
            {
                lengthOfVector = i;
                break;
            }
        }

        // shorten vector
        Array.Resize(ref globalVertexPositions, lengthOfVector);
        return globalVertexPositions;
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
