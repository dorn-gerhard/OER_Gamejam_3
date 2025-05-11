using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MathFunctionPlotter : MonoBehaviour
{
    public bool shoot;
    public bool printFunction;
    public bool displayOrigin;
    public List<float> showPoints;
    public float paramMin = -6;
    public float paramMax = 6;
    public float paramOrigin = 0;
    public float maxArcLength;
    public float totalArcLength;
    
    
    public MathFunction mathFunction;
    public LineRenderer lineRenderer;
    public LineRenderer originDot;
    public GameObject point;
    private GameObject points;

    public Vector3[] localCoordinates;
    public Vector3[] globalCoordinates;
    

    EdgeCollider2D edge;

    public Projectile projectile;
    public FunctionType currentWeaponType = 0;
    
    public bool drawNow;
    
    public int numberOfPoints = 600;
    private GameObject[] Points;
    
    



    // Start is called before the first frame update
    void Start()
    {
        edge = GetComponent<EdgeCollider2D>();
        maxArcLength = paramMax - paramMin;
        points = GameObject.Find("Points");
    }

    private void OnValidate()
    {
        if (drawNow)
        {
            DrawPoints();
            drawNow = false;
        }
        if (printFunction)
        {
            mathFunction.ShowDescription();
            printFunction = false;
        }
        if (shoot)
        {
            Shoot();
            shoot = false;
        }
    }

    private void Update()
    {
        Draw();
    }

    public Vector3[] Draw()
    {
        totalArcLength = 0;
        //Vector3[] vertexPositions = 
        localCoordinates = CalculateVertexPositions(paramOrigin);
        Vector3[] shiftedVertexPositions = ShiftVertexPositions(localCoordinates);

        DrawLine(localCoordinates.Length, localCoordinates);
        lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, 0.1f), new Keyframe(1, 0.1f));
        addCollider(shiftedVertexPositions.ToList());

        // display origin
        if (displayOrigin)
        {
            DrawPoint(paramOrigin, originDot);
        }
        else
        {
            originDot.enabled = false;    
        }

        // display points on curve

        DrawPoints();





        return localCoordinates;
    }

    

    void DrawPoints()
    {
        int numberOfPoints = points.transform.childCount;
        for (int k = 0; k < showPoints.Count; k++)
        {
            LineRenderer tempPoint;
            if (k >= numberOfPoints)
                tempPoint = Instantiate(point, points.transform).GetComponent<LineRenderer>();
            else
                tempPoint = points.transform.GetChild(k).GetComponent<LineRenderer>();
            DrawPoint(showPoints[k], tempPoint);
        }
        if (showPoints.Count < numberOfPoints)
        {
            for (int k = showPoints.Count; k < numberOfPoints; k++)
            {
                Destroy(points.transform.GetChild(k).gameObject);
            }
        }

    }
    
    public Vector3[] CalculateVertexPositions(float parameterOrigin = 0.0f)
    {
        

        Vector3[] vertexPositionsPositive = GetPath(parameterOrigin, paramMax, maxArcLength / 2);
        Vector3[] vertexPositionsNegative = GetPath(parameterOrigin, paramMin, maxArcLength / 2);

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
    private Vector3[] GetPath(float startValue, float endValue, float maxArcLength)
    {
        Vector3? temp;
        Vector3 point;
        float increment = (endValue - startValue) / (numberOfPoints + 1);
       
        Vector3[] tempCoordinates = new Vector3[numberOfPoints];
        int validPointIndex = 0;
        float length = 0.0f;
        for (int i = 0; i < numberOfPoints; i++)
        {
            temp = EvalFunction(startValue + i * increment);
            if (temp is not null)
            {
                point = (Vector3)temp;
                Vector3 temp_vector = new Vector3(point.x, point.y, point.z);
                tempCoordinates[validPointIndex] = temp_vector + transform.parent.transform.position;

                //Debug.Log($"index: {i}");
                if (validPointIndex > 0)
                {
                    length += (tempCoordinates[validPointIndex] - tempCoordinates[validPointIndex - 1]).magnitude;
                    //Debug.Log($"i: {i} arc length incremental {(tempCoordinates[validPointIndex] - tempCoordinates[validPointIndex - 1]).magnitude}, " +
                    //    $"first coordinate: {tempCoordinates[validPointIndex - 1].x}, second: {tempCoordinates[validPointIndex - 1].y}, " +
                    //    $"second coordinate: {tempCoordinates[validPointIndex].x}, second: {tempCoordinates[validPointIndex].y}");
                }

                validPointIndex++;

                if (length > maxArcLength)
                {
                    totalArcLength += length;
                    break;
                }
            }

        }

        // shorten vector
        Array.Resize(ref tempCoordinates, validPointIndex);
        return tempCoordinates;
    }
   

    public void Shoot()
    {
        //FreezeController.current.Unfreeze();
        //PlayerController.current.UnfreezeMovementAfterDelay();
        //PlayerController.current.ChangeEnergy(-PlayerController.current.energyPerShot);

       

        StartCoroutine(LaserBeam(1.0f));
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
        int k_final = 40;
        while (k < k_final)
        {
            yield return new WaitForSecondsRealtime(0.004f);
            //print("WaitAndPrint " + Time.unscaledTime);
            lineRenderer.widthCurve = GetLaserCurve((float)k / (2.0f + (float)k_final), 0.2f);
            k++;
        }
    }

    

    public LayerMask layerMask;   // The layer(s) to check for collisions
    

    float GetDistanceToEdge(Collider2D collider, EdgeCollider2D edge)
    {
        if (collider == null) return Mathf.Infinity;

        // Find the closest point on the edgeCollider and calculate the distance
        Vector2 closestPoint = edge.ClosestPoint(collider.transform.position);
        return Vector2.Distance(collider.transform.position, closestPoint);
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


        lineRenderer.SetPositions(vertexPositions);
    }
    void DrawPoint(float param, LineRenderer pointRenderer)
    {
        Vector3? pointVector = EvalFunction(param);
        if (pointVector is not null)
        {
            pointVector -= transform.parent.position;
            pointRenderer.enabled = true;
            pointRenderer.SetPositions(new Vector3[] { (Vector3)pointVector, (Vector3)pointVector });
        }
    }

    private Vector3? EvalFunction(float x)
    {
        return mathFunction.EvalFunc(x);
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Name of collider: " + collision.name);
        if (collision.gameObject.GetComponent<EnemyController>())
        {
            collision.gameObject.GetComponent<EnemyController>().health.Decrement(PlayerController.current.GetCurrentFunctionScriptableObject().type);
        }
    }


  
}
