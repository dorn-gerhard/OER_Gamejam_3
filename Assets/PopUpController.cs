using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpController : MonoBehaviour
{
    public static PopUpController current;

    public GameObject popUp;
    public GameObject joystick;

    public List<PopUpSettings> popUpSettings = new List<PopUpSettings>();

    public TMP_Text header;
    public TMP_Text body;

    public Button button;
    public TMP_Text buttonText;

    public Image image;

    PopUpSettings currentPopUpSettings;

    float prevTimeScale = 1;

    public bool DebugDisablePopups = false;

    List<string> opened = new List<string>();
    [SerializeField] List<GameObject> objectsToSetUnactive = new List<GameObject>();

    [Serializable]
    public struct PopUpSettings
    {
        public string tag;
        public string header;
        [Multiline]
        public string body;
        public Sprite sprite;

        public bool OpenOnce;
        public bool UnfreezeOnClose;
        //public float countdown;
        public bool resumePlayWithCountdown;
        public bool DontSetObjectActiveOnClose;
    }

    private void Awake()
    {
        current = this;
    }
    
    PopUpSettings Find(string tag)
    {
        foreach (PopUpSettings popUpSettings in popUpSettings)
        {
            if (popUpSettings.tag == tag) return popUpSettings;
        }

        return default;
    }

    public bool OpenPopUp(string tag)
    {
#if UNITY_EDITOR
        if (DebugDisablePopups) return false;
#endif
        //OnClosePopUp();

        currentPopUpSettings = Find(tag);

        if (currentPopUpSettings.OpenOnce && opened.Contains(currentPopUpSettings.tag))
        {
            print("already openeded before");
            return false;
        }

        if (currentPopUpSettings.tag == null) return false;

        prevTimeScale = Time.timeScale;

        Time.timeScale = 0;

        body.text = currentPopUpSettings.body;
        header.text = currentPopUpSettings.header;
        image.sprite = currentPopUpSettings.sprite;

        PopUpSettings result;
        foreach (PopUpSettings popUpSettings in popUpSettings)
        {
            if (popUpSettings.tag == tag)
            {
                result = popUpSettings;
                break;
            }
        }

        if (!opened.Contains(currentPopUpSettings.tag))
        {
            opened.Add(currentPopUpSettings.tag);
        }

        popUp.SetActive(true);
        joystick.SetActive(false);

        //if (currentPopUpSettings.countdown > 0)
        //{
        //    StartCoroutine(Countdown(currentPopUpSettings.countdown));
        //}
        return true;
    }

    //IEnumerator Countdown(float countdown)
    //{
    //    float currentCountdown = countdown;

    //    button.interactable = false;
    //    buttonText.text = currentCountdown.ToString("F0");

    //    while(currentCountdown > 0)
    //    {
    //        yield return new WaitForSecondsRealtime(1);
    //        currentCountdown--;
    //        buttonText.text = currentCountdown.ToString("F0");
    //    }

    //    button.interactable = true;
    //    buttonText.text = "Weiter";

    //    Time.timeScale = 1;

    //    OnClosePopUp();

    //    yield return null;
    //}

    public void OnClosePopUp()
    {
        popUp.SetActive(false);
        joystick.SetActive(true);
        joystick.GetComponent<FixedJoystick>();
        print("setting back to timescale");

        if (currentPopUpSettings.UnfreezeOnClose)
        {
            Time.timeScale = prevTimeScale;
        }

        if (!currentPopUpSettings.DontSetObjectActiveOnClose)
        {
            foreach (var item in objectsToSetUnactive)
            {
                item.SetActive(true);
            }
        }

        if (currentPopUpSettings.resumePlayWithCountdown)
        {
            CountdownController.current.ResumePlay();
        }
    }
}
