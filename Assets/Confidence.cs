using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Function;

public class Confidence : MonoBehaviour
{
    public FunctionScriptableObject functionScriptableObject;
    public float confidence = 0;
    public Image confidencefillBar;
    public TMP_Text confidenceProcentageText;
    public TMP_Text energyProcentageText;

    public Image energyWarningBackground;
    public Animator energyWarningAnimator;

    public GameObject energyBar;

    public GameObject switchWeaponIndicator;

    public float energy = 100;
    public Image energyfillBar;

    public float decayDelay = 3;
    public float decayAmount = 10;
    float timeSinceLastDelay = 0;

    bool lowEnergyWarning = false;

    Vector3 CalibrationTargetPositionCache = Vector3.zero;

    public Vector3 GetCalibrationTargetPositionCache()
    {
        return CalibrationTargetPositionCache;
    }

    public bool HasCalibrationTargetPositionCache()
    {
        return CalibrationTargetPositionCache != Vector3.zero;
    }

    public void SetCalibrationTargetPositionCache(Vector3 c)
    {
        CalibrationTargetPositionCache = c;
    }

    public void UpdateConfidenceUI()
    {
        confidencefillBar.fillAmount = confidence / 100;
        confidenceProcentageText.text = confidence.ToString("F0") + "%";
        energyProcentageText.text = energy.ToString("F0") + "%";

        energyfillBar.fillAmount = energy / 100;
    }

    public void OnSwitchWeapon()
    {
        PlayerController.current.OnSwitchWeapon(this);
    }

    private void Update()
    {
        Decay();

        if (!CalibrationController.current.IsCalibratingMinigameActive && energy <= 20)
        {
            if (lowEnergyWarning) return;
            lowEnergyWarning = true;

            energyWarningBackground.color = Color.red;
            energyWarningAnimator.enabled = true;

            if (PopUpController.current.OpenPopUp("lowenergy"))
            {
                CountdownController.current.ForceInstantResume();
            }
        }
        else
        {
            if (!lowEnergyWarning) return;
            lowEnergyWarning = false;

            energyWarningBackground.color = Color.white;
            energyWarningAnimator.enabled = false;
        }        
    }

    void Decay()
    {
        if (gameObject.activeSelf && gameObject.GetComponent<CanvasGroup>().alpha < 1)
        {
            timeSinceLastDelay += Time.deltaTime;
            if (timeSinceLastDelay >= decayDelay)
            {
                confidence -= decayAmount;
                confidence = Mathf.Clamp(confidence, 0, 100);
                UpdateConfidenceUI();
                timeSinceLastDelay = 0;
            }            
        }
    }

    //void ChangeConfidence
    //{

    //}
}
