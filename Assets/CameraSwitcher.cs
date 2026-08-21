using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("相机设置")]
    public Camera targetCamera;
    private Camera previousCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        previousCamera = Camera.main;

        Camera[] allCameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in allCameras)
        {
            cam.enabled = false;
        }

        targetCamera.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        targetCamera.enabled = false;

        if (previousCamera != null)
        {
            previousCamera.enabled = true;
        }
    }
}
