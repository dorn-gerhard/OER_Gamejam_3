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
    private Confidence currentConfidence;

    public void Setup(int index)
    {
        currentConfidence = confidences.transform.GetChild(index).GetComponent<Confidence>();

        image.sprite = currentConfidence.functionExampleImage;
        text.text = function.GetFunctionString(index);
    }

    public void OnClick()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}
