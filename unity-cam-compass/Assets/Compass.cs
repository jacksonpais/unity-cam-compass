using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    public GameObject MainCamera;

    WebCamTexture cam;
    bool isCamReady = false;
    public RawImage bg;
    public AspectRatioFitter fit;
    Texture defaultBg;

    private Gyroscope gyro;
    private Quaternion rotation;
    bool isGyroReady = false;

    void Start()
    {
        defaultBg = bg.texture;
        WebCamDevice[] devices = WebCamTexture.devices;
        if(devices.Length==0)
        {
            Debug.Log("No camera");
            isCamReady = false;
            return;
        }
        for (int i=0; i<devices.Length;i++)
        {
            if(!devices[i].isFrontFacing)
            {
                cam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if (cam == null)
        {
            Debug.Log("No back camera");
            isCamReady = false;
            return;
        }
        else
        {
            if (!cam.isPlaying)
                cam.Play();
            bg.texture = cam;
            isCamReady = true;
        }
        if (!SystemInfo.supportsGyroscope)
        {
            return;
        }
        gyro = Input.gyro;
        gyro.enabled = true;
        rotation = new Quaternion(0, 0, 1, 0);
        isGyroReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isCamReady)
        {
            return;
        }

        float ratio = (float)cam.width / (float)cam.height;
        fit.aspectRatio = ratio;

        float scaleY = cam.videoVerticallyMirrored ? -1f : 1f;
        bg.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
        int orientation = cam.videoRotationAngle;
        bg.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);

        if (isGyroReady)
        {
            transform.localRotation = new Quaternion(0, 0, -Input.gyro.attitude.z, Input.gyro.attitude.w);
        }
    }
}
