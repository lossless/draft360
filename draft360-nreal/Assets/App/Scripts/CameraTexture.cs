using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class CameraTexture : MonoBehaviour
{
    public static CameraTexture Instance;

    public WebCamTexture webcamTexture;
    RawImage cameraImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        cameraImage = GetComponent<RawImage>();
    }

    private void Start()
    {
        OpenCamera();
    }

    public void OpenCamera()
    {
        webcamTexture = new WebCamTexture();
        cameraImage.texture = webcamTexture;
        cameraImage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }
}
