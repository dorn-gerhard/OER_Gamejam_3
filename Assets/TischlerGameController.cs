using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TischlerGameController : MonoBehaviour
{
    public GameObject PlayerController; 
    public GameObject Line;

    public CutFunction cutFunction;

    public GameObject cutOptionsDisplay; 
    public GameObject cutDisplay; 
    public GameObject choosePartDisplay; 

    public GameObject resultDisplay;

    CutOption currentCutOption;

    public TMP_Text cutSelectionOptionDisplayText1; 
    public TMP_Text cutSelectionOptionDisplayText2; 
    public TMP_Text cutSelectionOptionDisplayText3; 

    [Serializable]
    public struct CutOption
    {
        public string displayText;
        public FunctionType functionType;
        public float par1;
        public float par2;
    }

    public List<CutOption> cutOptions = new List<CutOption>();

    public List<CutOption> currentCutSelection = new List<CutOption>();

    // Start is called before the first frame update
    void Start()
    {
        StartNewRound();
    }

    void StartNewRound()
    {
        Line.SetActive(false);

        choosePartDisplay.SetActive(false);
        resultDisplay.SetActive(false);
        cutDisplay.SetActive(false);

        PlayerController.SetActive(true);

        SetCutSelection();
    }

    private void SetCutSelection()
    {
        List<CutOption> shuffledList = new List<CutOption>(cutOptions);
        for (int i = 0; i < shuffledList.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, shuffledList.Count);
            // Swap elements
            CutOption temp = shuffledList[i];
            shuffledList[i] = shuffledList[randomIndex];
            shuffledList[randomIndex] = temp;
        }

        currentCutSelection.Clear();

        for (int i = 0; i < 3; i++)
        {
            currentCutSelection.Add(shuffledList[i]);
        }

        cutOptionsDisplay.SetActive(true);

        cutSelectionOptionDisplayText1.text = currentCutSelection[0].displayText;
        cutSelectionOptionDisplayText2.text = currentCutSelection[1].displayText;
        cutSelectionOptionDisplayText3.text = currentCutSelection[2].displayText;
    }

    public void SelectOption(int optionIndex)
    {
        currentCutOption = currentCutSelection[optionIndex];

        Line.SetActive(true);

        cutFunction.currentWeaponType = currentCutOption.functionType;
        cutFunction.par1 = currentCutOption.par1;
        cutFunction.par2 = currentCutOption.par2;

        cutOptionsDisplay.SetActive(false);

        cutDisplay.SetActive(true);
    }

    public void Cut()
    {
        // do we need to call cut directly here?
        cutDisplay.SetActive(false);
        cutFunction.fireNow = true;
    }

    void Compare()
    {

    }
}
