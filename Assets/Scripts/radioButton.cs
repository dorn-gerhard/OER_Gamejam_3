using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class radioButton : MonoBehaviour
{
    [SerializeField] private AudioSource BackgroundMusic;

    public void turnOff()
    {
        BackgroundMusic.volume =0f;
    }
}