using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;
using TMPro;
using TexDrawLib;
using Platformer.Mechanics;

public class Function : MonoBehaviour
{
    public LineRenderer lineRenderer;
    EdgeCollider2D edge;

    public Projectile projectile;
    public FunctionType currentWeaponType = 0;

    public int numberOfPoints = 600;
    public float par1 = 0.5f;
    public float par2 = 1f;

    public float ComboConfidenceGain = 3;

    [Header("Slider")]
    public Slider rotationSlider;
    public Slider parameter1;
    public Slider parameter2;
    public TEXDraw FunctionDynamicLabel;
    public TEXDraw LabelParameter1;
    public TEXDraw LabelParameter2;

    [Header("TextInput")]
    public TEXDraw TextInputParameter1;
    public TEXDraw TextInputParameter2;
    public TEXDraw TextInputParameter3;
    public TMP_InputField TextInputField1;
    public TMP_InputField TextInputField2;

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
        rotationSlider.minValue = -180;
        rotationSlider.maxValue = 180;

        // phase
        parameter1.minValue = -10;
        parameter1.maxValue = 10;
        // amplitude
        parameter2.minValue = -5;
        parameter2.maxValue = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (CalibrationController.current.IsCalibrating) return;

        Draw();
    }

    public void SetLineLength(LineLengthData newLineLengthData)
    {
        currentLineLengthData = newLineLengthData;
    }

    public void Shoot()
    {
        FreezeController.current.Unfreeze();
        PlayerController.current.UnfreezeMovementAfterDelay();
        PlayerController.current.ChangeEnergy(-PlayerController.current.energyPerShot);

        if (currentCombo > 1)
        {
            float comboGain = (float)Math.Pow(ComboConfidenceGain, currentCombo);
            print($"comboGain {comboGain}");

            PlayerController.current.ChangeConfidence(comboGain);
        }

        StartCoroutine(LaserBeam(1.0f));
    }

    AnimationCurve GetLaserCurve(float t, float width)
    {
        float height = 0.3f;
        if ( t < width/2)
            return new AnimationCurve(new Keyframe(0, 0),
                                      new Keyframe(0.5f - t - width/2, 0.05f),
                                      new Keyframe(0.5f - t, height),
                                      new Keyframe(0.5f, height * (width/2 - t)/width/2),
                                      new Keyframe(0.5f + t , height),
                                      new Keyframe(0.5f + t + width/2, 0.05f),
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
                yValue = EvalFunction(startValue + i * increment, par1, par2);
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
        GetDynamicFunctionString();
        return vertexPositions;
    }

    public LayerMask layerMask;   // The layer(s) to check for collisions
    int currentCombo;
    public Vector3[] Draw()
    {
        Vector3[] vertexPositions = CalculateVertexPositions();
        Vector3[] shiftedVertexPositions = ShiftVertexPositions(vertexPositions);

        DrawLine(vertexPositions.Length, vertexPositions);
        lineRenderer.widthCurve =  new AnimationCurve(new Keyframe(0, 0.1f), new Keyframe(1, 0.1f));
        addCollider(shiftedVertexPositions.ToList());

        if (!CalibrationController.current.IsCalibratingMinigameActive)
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(layerMask); // Filter collisions based on the layer mask
            Collider2D[] results = new Collider2D[20];

            edge.OverlapCollider(contactFilter, results);

            // Sort by distance (closest first)
            results = results.OrderBy(collider => GetDistanceToEdge(collider, edge)).ToArray();

            currentCombo = 0;
            foreach (Collider2D collider in results)
            {
                if (collider == null) continue;

                if (collider.gameObject.GetComponent<Health>().functionType == PlayerController.current.GetCurrentFunctionScriptableObject().type)
                {
                    currentCombo++;
                }

                collider.gameObject.GetComponent<EnemyController>().combo = currentCombo;

                collider.gameObject.GetComponent<EnemyController>().isInSights = true;
            }
        }

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

        lineRenderer.colorGradient = PlayerController.current.GetCurrentFunctionScriptableObject().gradient;

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

    public void UpdateParameter1(string newValue)
    {
        float result;
        if (TryParseInput(newValue, out result))
        {
            par1 = result;
        }        
    }
    public void UpdateParameter2(string newValue)
    {
        float result;
        if (TryParseInput(newValue, out result))
        {
            par2 = result;
        }
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

    public string GetDynamicFunctionString(bool setDynamicCalibrationFunctionString = false, float RandomPar1 = 0, float RandomPar2 = 0)
    {
        DynamicFunctionParts parts = GetDynamicFunctionStringParts(setDynamicCalibrationFunctionString, RandomPar1, RandomPar2);

        var text2 = parts.text2.Trim();
        if (text2.EndsWith("+"))
        {
            if (parts.input2.Trim().StartsWith("-"))
            {
                text2 = text2.TrimEnd("+");
            }
        }

        string functionString = $"${parts.text1}\\color[green]{parts.input1}\\color[black]{text2}\\color[orange]{parts.input2}\\color[black]{parts.text3}$";
        FunctionDynamicLabel.text = functionString;

        return functionString;
    }

    public DynamicFunctionParts GetDynamicFunctionStringParts(bool setDynamicCalibrationFunctionString, float RandomPar1, float RandomPar2)
    {
        DynamicFunctionParts dynamicFunctionParts = new DynamicFunctionParts();

        if (currentWeaponType == FunctionType.linear)
        {
            dynamicFunctionParts.text1 = "f(x) = ";
            dynamicFunctionParts.input1 = Math.Round(par1, 2).ToString();
            dynamicFunctionParts.text2 = " \\cdot x + ";
            dynamicFunctionParts.input2 = Math.Round(par2, 2).ToString();
            dynamicFunctionParts.text3 = "";

            LabelParameter1.text = "$k\\colon " + Math.Round(par1, 2) + "$";
            LabelParameter2.text = "$d\\colon " + Math.Round(par2, 2) + "$";
        }
        else if (currentWeaponType == FunctionType.quadratic)
        {
            dynamicFunctionParts.text1 = "f(x) = ";
            dynamicFunctionParts.input1 = Math.Round(par1, 2).ToString();
            dynamicFunctionParts.text2 = " \\cdot x^2 + ";
            dynamicFunctionParts.input2 = Math.Round(par2, 2).ToString();
            dynamicFunctionParts.text3 = "";

            LabelParameter1.text = "$a\\colon " + Math.Round(par1, 2) + "$";
            LabelParameter2.text = "$b\\colon " + Math.Round(par2, 2) + "$";
        }
        else if (currentWeaponType == FunctionType.sin)
        {
            dynamicFunctionParts.text1 = "f(x) = ";
            dynamicFunctionParts.input1 = Math.Round(par1, 2).ToString();
            dynamicFunctionParts.text2 = " \\cdot \\sin(";
            dynamicFunctionParts.input2 = Math.Round(par2, 2).ToString();
            dynamicFunctionParts.text3 = " \\cdot x)";

            LabelParameter1.text = "$a\\colon " + Math.Round(par1, 2) + "$";
            LabelParameter2.text = "$b\\colon " + Math.Round(par2, 2) + "$";
        }

        // TODO make this work with 2 input fields

        if (CalibrationController.current.IsCalibratingMinigameActive && setDynamicCalibrationFunctionString)
        {
            SetCalibrationDynamicFunctionString(dynamicFunctionParts, RandomPar1, RandomPar2);
        }

        return dynamicFunctionParts;
    }

    private void SetCalibrationDynamicFunctionString(DynamicFunctionParts dynamicFunctionParts, float randomPar1, float randomPar2)
    {
        float currentConfidence = PlayerController.current.currentConfidence;
        FunctionScriptableObject functionScriptableObject = PlayerController.current.GetCurrentFunctionScriptableObject();

        if (currentConfidence < 25.0f)
        {
            TextInputField1.gameObject.SetActive(true);
            TextInputField2.gameObject.SetActive(false);

            float currentValueInInputField;
            if (TryParseInput(TextInputField1.text, out currentValueInInputField))
            {
                par1 = currentValueInInputField;
            }
            else
            {
                par1 = functionScriptableObject.defaultPar1Value;
                TextInputField2.text = par1.ToString();
            }

            par2 = functionScriptableObject.defaultPar2Value; // TODO check if this needs to happen earlier

            TextInputParameter1.text = $"${dynamicFunctionParts.text1}$";
            TextInputParameter2.text = $"${dynamicFunctionParts.text2}$";
            TextInputParameter3.text = $"${par2}{dynamicFunctionParts.text3}$"; //TODO find better solution instead of just adding 0, maybe even second param random? (we could set par2 to the randomly generated value)
        }
        else if (currentConfidence < 50.0f)
        {
            TextInputField1.gameObject.SetActive(true);
            TextInputField2.gameObject.SetActive(false);

            float currentValueInInputField;
            if (TryParseInput(TextInputField1.text, out currentValueInInputField))
            {
                par1 = currentValueInInputField;
            }
            else
            {
                par1 = 0;
                TextInputField1.text = 0.ToString();
            }
            par2 = randomPar2;

            TextInputParameter1.text = $"${dynamicFunctionParts.text1}$";
            TextInputParameter2.text = $"${dynamicFunctionParts.text2}$";
            TextInputParameter3.text = $"${par2}{dynamicFunctionParts.text3}$"; //TODO find better solution instead of just adding 0, maybe even second param random? (we could set par2 to the randomly generated value)        
        }
        else if (currentConfidence < 75.0f)
        {
            TextInputField1.gameObject.SetActive(false);
            TextInputField2.gameObject.SetActive(true);

            par1 = functionScriptableObject.defaultPar1Value;

            float currentValueInInputField;
            if (TryParseInput(TextInputField2.text, out currentValueInInputField))
            {
                par2 = currentValueInInputField;
            }
            else
            {
                par2 = functionScriptableObject.defaultPar2Value;
                TextInputField2.text = par2.ToString();
            }

            TextInputParameter1.text = $"${dynamicFunctionParts.text1}{par1}$";
            TextInputParameter2.text = $"${dynamicFunctionParts.text2}$";
            TextInputParameter3.text = $"${dynamicFunctionParts.text3}$"; //TODO find better solution instead of just adding 0, maybe even second param random? (we could set par2 to the randomly generated value)
        }
        else
        {
            TextInputField1.gameObject.SetActive(false);
            TextInputField2.gameObject.SetActive(true);

            par1 = randomPar1;

            float currentValueInInputField;
            if (TryParseInput(TextInputField2.text, out currentValueInInputField))
            {
                par2 = currentValueInInputField;
            }
            else
            {
                par2 = functionScriptableObject.defaultPar2Value;
                TextInputField2.text = par2.ToString();
            }

            TextInputParameter1.text = $"${dynamicFunctionParts.text1}{par1}$";
            TextInputParameter2.text = $"${dynamicFunctionParts.text2}$";
            TextInputParameter3.text = $"${dynamicFunctionParts.text3}$"; //TODO find better solution instead of just adding 0, maybe even second param random? (we could set par2 to the randomly generated value)
        }      
    }

    public static bool TryParseInput(string input, out float result)
    {
        input = input.Replace(".", ",").Replace("\\", "/");

        if (float.TryParse(input, out result))
        {
            return true;
        }
        else if (input.Contains("/"))
        {
            var split = input.Split("/");

            if (split.Length == 2)
            {
                float left;
                float right;
                if (float.TryParse(split[0], out left) == false) return false;
                if (float.TryParse(split[1], out right) == false) return false;

                if (right == 0)
                {
                    return false;
                }

                result = left / right;

                return true;
            }
        }

        return false;
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
