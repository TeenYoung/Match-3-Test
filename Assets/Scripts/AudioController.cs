using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource efxSource, bgmSource;
    public static AudioController audioController = null;


    private void Awake()
    {
        if (audioController == null)
        {
            audioController = this;
        }
        else if (audioController != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySoungEffect(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

}
