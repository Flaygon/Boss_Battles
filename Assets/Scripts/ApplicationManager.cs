using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public AudioSource backgroundMusic;

    private void Awake()
    {
        instance = this;
    }

    public void PlayMusic(AudioClip music)
    {
        if(backgroundMusic.clip == null)
        {
            backgroundMusic.clip = music;

            if (backgroundMusic.clip != null)
                backgroundMusic.Play();
        }
        else if(backgroundMusic.clip != music)
        {
            backgroundMusic.Stop();
            backgroundMusic.clip = music;

            if(backgroundMusic.clip != null)
                backgroundMusic.Play();
        }
    }

    private static ApplicationManager instance;
    public static ApplicationManager Get()
    {
        if (instance == null)
        {
            GameObject newInstance = new GameObject();
            DontDestroyOnLoad(newInstance);
            newInstance.AddComponent<ApplicationManager>();
            newInstance.AddComponent<AudioSource>();
            newInstance.GetComponent<AudioSource>().loop = true;
            newInstance.GetComponent<ApplicationManager>().backgroundMusic = newInstance.GetComponent<AudioSource>();
            instance = newInstance.GetComponent<ApplicationManager>();
        }
        return instance;
    }
}