using System;
using System.IO;
using UnityEngine;

public class ImageCapture : MonoBehaviour
{
    public Camera droneCamera;
    public RenderTexture renderTexture;
    public float captureInterval = 1f; // Time in seconds between captures  
    public int maxCaptures = 1000; // Maximum number of captures

    private float timer = 0f;
    private int imageCount = 0;
    private string savePath;

    void Start()
    {
        if (droneCamera == null || renderTexture == null)
        {
            Debug.LogError("Camera or RenderTexture is not assigned.");
            enabled = false;
            return;
        }

        savePath = Path.Combine(Application.dataPath, "../CapturedImages");
        if (!Directory.Exists(savePath))
        {
            try
            {
                Directory.CreateDirectory(savePath);
                Debug.Log("Directory created at: " + Path.GetFullPath(savePath));
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to create directory: " + e.Message);
                enabled = false;
                return;
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= captureInterval && imageCount < maxCaptures)
        {
            CaptureImage();
            timer = 0f;
            imageCount++;
            Debug.Log("Captured image " + imageCount + " at " + Time.time);
        }
        else if (imageCount >= maxCaptures)
        {
            Debug.Log("Maximum number of captures reached.");
            enabled = false;
        }
    }

    void CaptureImage()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;
        droneCamera.targetTexture = renderTexture;

        droneCamera.Render();

        Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        image.Apply();

        string filename = Path.Combine(savePath, $"img_{imageCount:D4}.png");
        try
        {
            File.WriteAllBytes(filename, image.EncodeToPNG());
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save image: " + e.Message);
        }

        RenderTexture.active = currentRT;
        droneCamera.targetTexture = null;
        Destroy(image);
    }
}
