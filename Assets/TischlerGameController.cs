using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TischlerGameController : MonoBehaviour
{
    public GameObject playerController; 
    public GameObject Line;

    public CutFunction cutFunction;

    public GameObject cutOptionsDisplay; 
    public GameObject cutDisplay; 
    public GameObject choosePartDisplay; 

    public GameObject resultDisplay;
    public TMP_Text resultDisplayText;

    CutOption currentCutOption;

    public TMP_Text cutSelectionOptionDisplayText1; 
    public TMP_Text cutSelectionOptionDisplayText2; 
    public TMP_Text cutSelectionOptionDisplayText3;

    public Image cutSelectionImage1;
    public Image cutSelectionImage2;
    public Image cutSelectionImage3;

    public static TischlerGameController current;

    public bool doSelection = false;

    public TMP_Text cutTryCounterDisplay;

    public static int currentLevel = 1;

    [Serializable]
    public struct CutOption
    {
        public Sprite sprite;
        public string displayText;
        public FunctionType functionType;
        public float par1;
        public float par2;
    }

    [Serializable]
    public struct Goal
    {
        public int cutTries;
        public List<Vector2> points;
    }

    public List<Goal> goals = new List<Goal>();

    Goal currentGoal;

    int currentCutTriesLeft = 0;

    public List<CutOption> cutOptions = new List<CutOption>();

    public List<CutOption> currentCutSelection = new List<CutOption>();

    // Start is called before the first frame update
    void Start()
    {
        current = this;

        currentGoal = goals[0];
        //currentGoal = goals[UnityEngine.Random.Range(1, goals.Count - 1)];

        //var brett = FindObjectOfType<PolygonSelecter>().GetComponent<Polygon>();
        //brett.InitBoard(-3, 3, 500, 0, 8);
        //brett.SetPolygon();

        StartNewRound();


    }

    public void StartNewRound()
    {
        doSelection = false;

        currentCutTriesLeft = currentGoal.cutTries;

        cutTryCounterDisplay.text = currentCutTriesLeft.ToString();

        Line.SetActive(false);

        choosePartDisplay.SetActive(false);
        resultDisplay.SetActive(false);
        cutDisplay.SetActive(false);

        playerController.SetActive(false);
        playerController.GetComponent<PlayerMove2D>().fixedJoystick.gameObject.SetActive(false);

        StartCutSelection();
    }

    public void SelectPolygon(GameObject selectedPolygon)
    {
        doSelection = false;

        foreach (var selector in FindObjectsOfType<PolygonSelecter>())
        {
            if (selector.gameObject == selectedPolygon) continue;

            Destroy(selector.gameObject);
        }

        choosePartDisplay.SetActive(false);

        StartCutSelection();
    }

    private void StartCutSelection()
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

        //cutSelectionOptionDisplayText1.text = currentCutSelection[0].displayText;
        //cutSelectionOptionDisplayText2.text = currentCutSelection[1].displayText;
        //cutSelectionOptionDisplayText3.text = currentCutSelection[2].displayText;

        cutSelectionImage1.sprite = currentCutSelection[0].sprite;
        cutSelectionImage2.sprite = currentCutSelection[1].sprite;
        cutSelectionImage3.sprite = currentCutSelection[2].sprite;
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

        playerController.SetActive(true);
        playerController.GetComponent<PlayerMove2D>().enabled = true;
        playerController.GetComponent<PlayerMove2D>().fixedJoystick.gameObject.SetActive(true);
    }

    public void Cut()
    {
        playerController.SetActive(false);
        playerController.GetComponent<PlayerMove2D>().fixedJoystick.gameObject.SetActive(false);

        StartCoroutine(CutRoutine());
    }


    public IEnumerator CutRoutine()
    {
        //stop movement
        playerController.SetActive(true);

        playerController.GetComponent<PlayerMove2D>().enabled = false;

        // do we need to call cut directly here?
        cutDisplay.SetActive(false);

        yield return cutFunction.Cut();

        //bool IsStillPossible = Compare() >= 0.8;
        bool IsStillPossible = true;

        Line.SetActive(false);

        playerController.GetComponent<PlayerMove2D>().enabled = false;
        playerController.GetComponent<PlayerMove2D>().fixedJoystick.gameObject.SetActive(false);

        playerController.SetActive(false);

        if (true)
        {
            choosePartDisplay.SetActive(true);
            doSelection = true;
            //StartCutSelection();

            if (currentLevel >= goals.Count - 1)
            {
                resultDisplayText.text = "Alle Kunden sind zufrieden, Feierabend!";
                resultDisplay.SetActive(true);
                // finished game
            }
            else
            {
                currentLevel++;

                resultDisplayText.text = "Sehr gut!";
                resultDisplay.SetActive(true);
            }
        }
        else
        {
            string failText = "";

            switch (UnityEngine.Random.Range(0, 3))
            {
                case 0:
                    failText = "Das wird nix mehr...";
                    break;

                case 1:
                    failText = "Jo, da kann i jetzt a nix mehr mochen...";
                    break;

                case 2:
                    failText = "Brauchst a Zuckal?";
                    break;

                default:
                    failText = "Das wird nix mehr...";
                    break;

            }

            resultDisplayText.text = failText;
            resultDisplay.SetActive(true);
        }

        yield return null;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    void Compare()
    {

    }
}
