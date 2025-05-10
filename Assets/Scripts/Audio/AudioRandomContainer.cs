using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRandomContainer : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private bool playContinuously = false;
    private AudioSource audioSource;
    private bool isPlaying = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = null;
    }

    void Update()
    {
        if (playContinuously && !isPlaying && !audioSource.isPlaying)
        {
            PlayRandomClip();
        }
        isPlaying = audioSource.isPlaying;
    }
    
    public void Play()
    {
        playContinuously = false;
        PlayRandomClip();
    }

    public void PlayContinuously()
    {
        playContinuously = true;
    }

    public void Stop()
    {
        playContinuously = false;
    }

    private void PlayRandomClip()
    {
        if (audioClips.Length == 0)
        {
            Debug.LogWarning("No audio clips assigned to the RandomAudioPlayer on " + gameObject.name);
            return;
        }

        int randomIndex = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[randomIndex];
        audioSource.Play();
    }
}
