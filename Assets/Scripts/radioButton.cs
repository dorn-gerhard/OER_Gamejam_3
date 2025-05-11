using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class radioButton : MonoBehaviour
{
    [SerializeField] private AudioSource BackgroundMusic;
    private bool music_is_off;
    private EventSystem eventSystem;
    private void Start()
    {
        eventSystem = EventSystem.current;
    }
    public void turnOff()
    {
        eventSystem.SetSelectedGameObject(null);
        if (!music_is_off)
        {
            BackgroundMusic.volume = 0f;
            music_is_off = true;
        }
        else
        {
            BackgroundMusic.volume = 1f;
            music_is_off=false;
        }

    }
}