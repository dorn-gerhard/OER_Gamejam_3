using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LvlUpUI : MonoBehaviour
{
    public Image image;
    public TMP_Text text;
    public Function function;

    public GameObject confidences;

    [SerializeField] List<GameObject> objectsToSetUnactive = new List<GameObject>();

    private Confidence currentConfidence;

    public void Setup(Confidence newConfidence)
    {
        currentConfidence = newConfidence;

        image.sprite = currentConfidence.functionScriptableObject.sprite;

        text.text = function.GetFunctionString(currentConfidence.functionScriptableObject.type);
    }

    public void OnClick()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Time.timeScale = 0;

        foreach (var item in objectsToSetUnactive)
        {
            item.SetActive(false);
        }
    }

    private void OnDisable()
    {
        //PopUpController.current.OpenPopUp("resumeshipcontrolls");
        CountdownController.current.ResumePlay();
    }
}
