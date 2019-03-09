using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clearPieces, swapBack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SEClearPieces()
    {
        audioSource.clip = clearPieces;
        audioSource.Play();
    }

    public void SESwapBack()
    {
        audioSource.clip = swapBack;
        audioSource.Play();
    }
}
