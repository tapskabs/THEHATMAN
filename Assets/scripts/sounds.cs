using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sounds : Behaviour
{
    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource or audio clip not properly set up.");
        }
    }
}

