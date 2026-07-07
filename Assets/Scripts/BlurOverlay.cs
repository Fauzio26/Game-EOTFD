using UnityEngine;
using UnityEngine.UI;

public class BlurOverlay : MonoBehaviour
{
    [Header("Blur Settings")]
    public float blurSize = 3f;
    public int iterations = 3;

    private RawImage rawImage;
    private RenderTexture renderTexture;
    private Camera mainCam;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        mainCam  = Camera.main;
        CaptureAndBlur();
    }

    void OnEnable()
    {
        CaptureAndBlur();
    }

    void CaptureAndBlur()
    {
        if (mainCam == null) return;

        int w = Screen.width;
        int h = Screen.height;

        // Capture layar
        RenderTexture rt = new RenderTexture(w, h, 0);
        mainCam.targetTexture = rt;
        mainCam.Render();
        mainCam.targetTexture = null;

        // Blur dengan downscale-upscale
        RenderTexture blurred = rt;
        for (int i = 0; i < iterations; i++)
        {
            RenderTexture temp = RenderTexture.GetTemporary(
                blurred.width / 2, blurred.height / 2);
            Graphics.Blit(blurred, temp);
            if (blurred != rt) RenderTexture.ReleaseTemporary(blurred);
            blurred = temp;
        }
        for (int i = 0; i < iterations; i++)
        {
            RenderTexture temp = RenderTexture.GetTemporary(
                blurred.width * 2, blurred.height * 2);
            Graphics.Blit(blurred, temp);
            RenderTexture.ReleaseTemporary(blurred);
            blurred = temp;
        }

        rawImage.texture = blurred;
        rawImage.color   = new Color(1, 1, 1, 0.85f);
    }
}