using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    public static CountdownController current;

    public GameObject countdownUI;
    public TMP_Text displayText;
    public TMP_Text number;

    public float countSpeed = 0.25f;

    [SerializeField] List<GameObject> objectsToSetUnactive = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        current = this;
    }

    public void ResumePlay()
    {
        foreach (var item in objectsToSetUnactive)
        {
            item.SetActive(true);
        }

        countdownUI.SetActive(true);
        StartCoroutine(TimeWillResumeCountdown(3f));
    }

    public IEnumerator TimeWillResumeCountdown(float countdown)
    {
        float currentCountdown = countdown;

        number.text = currentCountdown.ToString("F0");

        while (currentCountdown > 0)
        {
            Time.timeScale = 0;

            yield return new WaitForSecondsRealtime(countSpeed);
            currentCountdown--;
            number.text = currentCountdown.ToString("F0");
        }

        Time.timeScale = 1;

        Close();

        yield return null;
    }

    void Close()
    {
        countdownUI.SetActive(false);
    }
    public void ForceInstantResume()
    {
        foreach (var item in objectsToSetUnactive)
        {
            item.SetActive(true);
        }

        Close();
    }
}
