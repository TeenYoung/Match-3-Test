using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (Screen.currentResolution.height/ Screen.currentResolution.width > 2)
        {
            mainCamera.orthographicSize = 7;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
