using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muter : MonoBehaviour
{
    public List<AudioSource> audioSources = new List<AudioSource>();

    public GameObject MutedOverlay;

    bool muted = false;

    public void OnToggleMute()
    {
        muted = !muted;

        MutedOverlay.SetActive(muted);

        foreach (var audioSource in audioSources)
        {
            audioSource.mute = muted;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
