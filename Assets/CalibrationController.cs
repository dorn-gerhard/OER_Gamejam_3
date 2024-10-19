using System;
using System.Collections;
using System.Collections.Generic;
using TexDrawLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Function;

public class CalibrationController : MonoBehaviour
{
    public static CalibrationController current;

    [SerializeField] List<GameObject> objectsToSetActive = new List<GameObject>();
    [SerializeField] List<GameObject> objectsToSetUnactive = new List<GameObject>();

    public GameObject recalibrationWarning;

    [Header("Target")]
    public GameObject CalibrationTarget;

    public TEXDraw calibrationTargetPointValueText;

    public GameObject ClosestPoint;
    public GameObject CalibrationResultDisplay;

    public Button calibrateButton;

    [SerializeField] float targetMinDistance = 3;
    [SerializeField] float targetMaxDistance = 5;

    public bool IsCalibrating = false;
    public bool IsCalibratingMinigameActive = false;

    public LineLengthData calibrationLineLengthData;

    bool prepareForLvlUp = false;

    [Serializable]
    public struct CalibrationResult
    {
        public string displayText;
        public float upperCutoff;
        public bool isPassing;
        public float confidenceGain;
        public float energyGain;
        public Sprite sprite;
    }

    public List<CalibrationResult> calibrationResults = new List<CalibrationResult>();

    [SerializeField] Function function;

    // Start is called before the first frame update
    void Awake()
    {
        current = this;

        calibrationResults.Sort((x, y) => x.upperCutoff.CompareTo(y.upperCutoff));
    }

    private void Update()
    {
        if (!IsCalibrating) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            OnCalibrateShot();
        }
    }

    const float NOT_INIT_FLOAT = 999;

    float RandomParam1 = NOT_INIT_FLOAT;
    float RandomParam2 = NOT_INIT_FLOAT;

    public void StartCalibration()
    {
        PopUpController.current.OpenPopUp("calib");

        PlayerController.current.ResetLowEnergyWarningAnimators();

        IsCalibrating = true;
        IsCalibratingMinigameActive = true;

        prepareForLvlUp = false;

        recalibrationWarning.SetActive(false);

        //function.GetDynamicFunctionString();
        Time.timeScale = 0;

        FreezeController.current.ResetFreezeTimer();

        foreach (var item in objectsToSetActive)
        {
            item.SetActive(true);
        }
        foreach (var item in objectsToSetUnactive)
        {
            item.SetActive(false);
        }

        if (RandomParam1 == NOT_INIT_FLOAT)
        {
            FunctionScriptableObject functionScriptableObject = PlayerController.current.GetCurrentFunctionScriptableObject();
            RandomParam1 = Mathf.Round(UnityEngine.Random.Range(functionScriptableObject.par1Range.x, functionScriptableObject.par1Range.y) * 2) / 2.0f;
        }

        if (RandomParam2 == NOT_INIT_FLOAT)
        {
            FunctionScriptableObject functionScriptableObject = PlayerController.current.GetCurrentFunctionScriptableObject();
            RandomParam2 = Mathf.Round(UnityEngine.Random.Range(functionScriptableObject.par2Range.x, functionScriptableObject.par2Range.y) * 2) / 2.0f;
        }

        function.GetDynamicFunctionString(true, RandomParam1, RandomParam2);

        function.SetLineLength(calibrationLineLengthData);

        CalculateTargetPosition();

        function.gameObject.GetComponent<LineRenderer>().enabled = false;
    }


    private void CalculateTargetPosition()
    {
        // Get the player's current position
        Vector3 playerPosition = PlayerController.current.transform.position;

        Vector3 roundedRandomShiftedInRangeVector;
        if (PlayerController.current.GetCurrentConfidenceUI().HasCalibrationTargetPositionCache())
        {
            roundedRandomShiftedInRangeVector = PlayerController.current.GetCurrentConfidenceUI().GetCalibrationTargetPositionCache();
        }
        else
        {       
            Vector3[] unshiftedVertexPositions = SimulateShot();
            Vector3[] shiftedVertexPositions = function.ShiftVertexPositions(unshiftedVertexPositions);

            List<Vector3> unshiftedVertexPositionsInRange = new List<Vector3>();
            List<Vector3> shiftedPositionsInRange = new List<Vector3>();

            float distance = 0;
            Vector3 unshiftedPos;
            Vector3 pos;
            for (int i = 0; i < shiftedVertexPositions.Length; i++)
            {
                unshiftedPos = unshiftedVertexPositions[i];
                pos = shiftedVertexPositions[i];

                distance = Vector3.Distance(playerPosition, unshiftedVertexPositions[i]);

                if (distance >= targetMinDistance && distance <= targetMaxDistance)
                {
                    shiftedPositionsInRange.Add(pos);
                    unshiftedVertexPositionsInRange.Add(unshiftedPos);
                }
            }

            Vector3 randomShiftedInRangeVector = shiftedPositionsInRange[UnityEngine.Random.Range(0, shiftedPositionsInRange.Count)];
            roundedRandomShiftedInRangeVector = new Vector3(RoundToNearestHalf(randomShiftedInRangeVector.x), RoundToNearestHalf(randomShiftedInRangeVector.y), 0);

            PlayerController.current.GetCurrentConfidenceUI().SetCalibrationTargetPositionCache(roundedRandomShiftedInRangeVector);
                //Vector3 randomInRangeVector = vertexPositionsInRange[randomIndex];
                //Vector3 targetPosition = new Vector3((float)Math.Round(randomInRangeVector.x * 2) / 2, (float)Math.Round(randomInRangeVector.y * 2) / 2, 0);
        }

        calibrationTargetPointValueText.text = $"$$({roundedRandomShiftedInRangeVector.x}|{roundedRandomShiftedInRangeVector.y})$$";

        // Move the CalibrationTarget to the new position
        CalibrationTarget.transform.position = playerPosition + roundedRandomShiftedInRangeVector;
    }

    private Vector3[] SimulateShot()
    {
        var originalPar1 = function.par1;
        var originalPar2 = function.par2;

        float currentConfidence = PlayerController.current.currentConfidence;
        FunctionScriptableObject functionScriptableObject = PlayerController.current.GetCurrentFunctionScriptableObject();

        if (currentConfidence < 50.0f)
        {
            function.par1 = RoundToNearestHalf(UnityEngine.Random.Range(functionScriptableObject.par1Range.x, functionScriptableObject.par1Range.y));
        }
        else
        {
            function.par2 = RoundToNearestHalf(UnityEngine.Random.Range(functionScriptableObject.par2Range.x, functionScriptableObject.par2Range.y));
        }

        var vP = function.CalculateVertexPositions(); // TODO check if we need to call draw without it drawing actually and just returning vertexPositions

        function.par1 = originalPar1;
        function.par2 = originalPar2;

        return vP;
    }

    float RoundToNearestHalf(float f)
    {
        return (float)Math.Round(f * 2.0f) / 2.0f;
    }

    public void OnCalibrateShot()
    {
        if (TextInputIsValid())
        {
            StartCoroutine(CalibrateCoroutine());
        }        
    }

    private bool TextInputIsValid()
    {
        if (function.TextInputField1.gameObject.activeSelf)
        {
            float result;
            if (TryParseInput(function.TextInputField1.text, out result) == false)
            {
                PopUpController.current.OpenPopUp("inputvalidationfailed");
                return false;
            }
        }

        if (function.TextInputField2.gameObject.activeSelf)
        {
            float result;
            if (TryParseInput(function.TextInputField2.text, out result) == false)
            {
                PopUpController.current.OpenPopUp("inputvalidationfailed");
                return false;
            }
        }

        return true;
    }

    private IEnumerator CalibrateCoroutine()
    {
        IsCalibrating = false;

        calibrateButton.interactable = false;

        function.gameObject.GetComponent<LineRenderer>().enabled = true;

        PlayerController.current.PlaySound("sfx_lazer1");
        PlayerController.current.PlaySound("laserpulse");

        List<Vector3> vertexPositions = new List<Vector3>(function.Draw());

        yield return StartCoroutine(function.LaserBeam(1.0f));

        float minDistance = GetMinDistanceToTarget(vertexPositions);
        print("Min distance: " + minDistance);

        minDistance = (float)Math.Round(minDistance, 2);

        ClosestPoint.SetActive(true);

        if (calibrationResults.Count < 2)
        {
            Debug.LogError("Not enough calibration results set in calibration controller.");
        }

        // If we can't find a calibrationResult with a small enoug UpperCutoff we default to the one with the biggest cutoff (elements are sorted by upperCutoff in Awake())
        CalibrationResult calibrationResult = calibrationResults[calibrationResults.Count - 1];
        foreach (var item in calibrationResults)
        {
            if (minDistance <= item.upperCutoff)
            {
                calibrationResult = item;
                break;
            }
        }

        CalibrationResultDisplay.GetComponentInChildren<TMP_Text>().text = $"{calibrationResult.displayText} (Distanz: {minDistance.ToString("F2")})";
        CalibrationResultDisplay.GetComponentInChildren<Image>().sprite = calibrationResult.sprite;
        CalibrationResultDisplay.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);

        PlayerController.current.ChangeEnergy(calibrationResult.energyGain);

        if (PlayerController.current.currentConfidence + calibrationResult.confidenceGain >= 100)
        {
            prepareForLvlUp = true;
        }

        PlayerController.current.ChangeConfidence(calibrationResult.confidenceGain);

        ClosestPoint.SetActive(false);
        CalibrationResultDisplay.SetActive(false);

        calibrateButton.interactable = true;

        if (calibrationResult.isPassing)
        {
            StopCalibration();
        }
        else
        {
            // TODO reduce current weapon confidence
            PlayerController.current.GetCurrentConfidenceUI().SetCalibrationTargetPositionCache(Vector3.zero);
            StartCalibration();
        }

        yield return null;
    }

    private float GetMinDistanceToTarget(List<Vector3> vertexPositions)
    {
        Vector3 targetPosition = CalibrationTarget.transform.position;
        float minDistance = Mathf.Infinity;

        float currentDistance;
        Vector3 closestVector = Vector3.zero;
        foreach (Vector3 vertexPosition in vertexPositions)
        {
            currentDistance = Vector3.Distance(targetPosition, vertexPosition);
            if (currentDistance < minDistance)
            {
                closestVector = vertexPosition;
                minDistance = currentDistance;
            }
        }

        ClosestPoint.transform.position = closestVector;

        return minDistance;
    }

    public void StopCalibration()
    {
        function.gameObject.GetComponent<LineRenderer>().enabled = true;

        foreach (var item in objectsToSetActive)
        {
            item.SetActive(false);
        }
        foreach (var item in objectsToSetUnactive)
        {
            item.SetActive(true);
        }

        IsCalibrating = false;
        IsCalibratingMinigameActive = false;

        if (!prepareForLvlUp)
        {
            //PopUpController.current.OpenPopUp("resumeshipcontrolls");
            CountdownController.current.ResumePlay();
        }

        PlayerController.current.ClearAllCalibrationTargetPositionCaches();
        RandomParam1 = NOT_INIT_FLOAT;
        RandomParam2 = NOT_INIT_FLOAT;
    }
}
