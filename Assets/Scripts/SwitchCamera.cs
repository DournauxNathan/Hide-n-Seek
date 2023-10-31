using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public GameObject[] cameras;
    private int currentCameraIndex = 0;

    private void Start()
    {
        // Deactivate all cameras except the initial one
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(i == 0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // Deactivate the current camera
            cameras[currentCameraIndex].SetActive(false);

            // Switch to the next camera
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;

            // Activate the new camera
            cameras[currentCameraIndex].SetActive(true);
        }
    }
}
