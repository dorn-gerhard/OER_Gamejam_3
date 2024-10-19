using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Function;

public class FreezeController : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToSetActiveOnFreeze = new List<GameObject>();
    [SerializeField] List<GameObject> objectsToSetUnactiveOnFreeze = new List<GameObject>();

    [SerializeField] float freezeDelay = 5;

    [SerializeField] GameObject calibrateYourWeaponInfoUI;

    [Range(0f, 1f)]
    public float minEnergyLineLengthProportion = 0.4f;

    float timeSinceUnfrozen = 0;

    bool isFrozen = false;

    public static FreezeController current;

    public Function function;

    public Button shootButton;
    public TMP_Text shootButtonText;

    public Image laserFillBar;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        timeSinceUnfrozen = 0;  
    }

    public void ResetFreezeTimer()
    {
        timeSinceUnfrozen = 0;
    }

    void Update()
    {
        if (CalibrationController.current.IsCalibratingMinigameActive) return;

        if (PlayerController.current.currentEnergy < PlayerController.current.energyPerShot)
        {
            if (!calibrateYourWeaponInfoUI.activeSelf) calibrateYourWeaponInfoUI.SetActive(true);

            if (isFrozen)
            {
                shootButton.interactable = false;
                shootButtonText.text = "Zu wenig Energie!";
            }

            return;
        }
        else
        {
            if (calibrateYourWeaponInfoUI.activeSelf) calibrateYourWeaponInfoUI.SetActive(false);

            if (isFrozen)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
                {
                    function.Shoot();
                }

                shootButton.interactable = true;
                shootButtonText.text = "Feuer!";
            }
        }

        timeSinceUnfrozen += Time.deltaTime;

        laserFillBar.fillAmount = timeSinceUnfrozen / freezeDelay;

        if (timeSinceUnfrozen >= freezeDelay)
        {
            Freeze();
        }
    }

    public void Freeze()
    {
        isFrozen = true;

        Time.timeScale = 0;

        PlayerController.current.Freeze();

        PlayerController.current.ResetLowEnergyWarningAnimators();

        foreach (GameObject obj in objectsToSetActiveOnFreeze)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToSetUnactiveOnFreeze)
        {
            obj.SetActive(false);
        }

        SetLineLength();
    }

    public void SetLineLength()
    {
        LineLengthData currentWeaponLineLength = PlayerController.current.GetCurrentWeaponLineLength();

        float currentEnergy = PlayerController.current.currentEnergy;

        float currentEnergyProportion = Math.Clamp(currentEnergy / 100, minEnergyLineLengthProportion, 1f);

        LineLengthData energyAdaptedLineLengthData = new LineLengthData();

        energyAdaptedLineLengthData.startValue = currentWeaponLineLength.startValue * currentEnergyProportion;
        energyAdaptedLineLengthData.endValue = currentWeaponLineLength.endValue * currentEnergyProportion;
        energyAdaptedLineLengthData.maxLength = currentWeaponLineLength.maxLength * currentEnergyProportion;

        function.SetLineLength(energyAdaptedLineLengthData);
    }

    public void Unfreeze()
    {
        isFrozen = false;
        timeSinceUnfrozen = 0;

        //PlayerController.current.UnFreeze();
        //foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
        //{
        //    enemy.UnFreeze();
        //}

        foreach (GameObject obj in objectsToSetActiveOnFreeze)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in objectsToSetUnactiveOnFreeze)
        {
            obj.SetActive(true);
        }

        //PopUpController.current.OpenPopUp("resumeshipcontrolls");
        CountdownController.current.ResumePlay();
    }
}
