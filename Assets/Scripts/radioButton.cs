using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class radioButton : MonoBehaviour
{
    [SerializeField] private AudioSource BackgroundMusic;

    public void turnOff()
    {
        if (BackgroundMusic.volume == 0f)
        {
            BackgroundMusic.volume =0f;
        }
        else
        {
           BackgroundMusic.volume =0f;
        }
        
    }
}