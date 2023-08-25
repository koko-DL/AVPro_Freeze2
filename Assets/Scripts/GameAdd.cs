using UnityEngine;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using RenderHeads.Media.AVProMovieCapture;

public class GameAdd : MonoBehaviour
{
    public CaptureFromTexture captureFromTexture;
    public RenderTexture movieRenderTexture;
    public UnityAudioCapture audioCapture;

    private float deltaTime;
    private float fps;
    private Texture2D texture;
    // Use this for initialization
    void Start () {
        //Screen.SetResolution(1280, 720, true);

        // ディスプレイを有効化
        /*
        for (int i = (int)Display.displays.Length - 1; i >= 0; i--)
        {
            //Screen.fullScreen = false;
            //Display.displays[i].Activate(1280, 720, 60);
            Display.displays[i].Activate();
        }
        */
        if (Display.displays.Length > 1)
        {
            Display.displays[1].SetParams(1280, 720, 0, 0);
            Display.displays[1].Activate(1280, 720, 60);
        }

        Display.displays[0].SetParams(1280, 720, 0, 0);
        Display.displays[0].Activate(1280, 720, 60);

        texture = new Texture2D(1, 1);

        // Generate a solid color for the texture (you can modify this part as needed)
        Color32[] colors = new Color32[1];
        Color32 color = new Color32(0, 0, 0, 128); // Red color
        colors[0] = color;
        
        // Set the pixel data of the texture
        texture.SetPixels32(colors);

        // Apply the changes to the texture
        texture.Apply();

        var hints = captureFromTexture.GetEncoderHints();
        hints.videoHints.maximumBitrate = 2000000;
        hints.videoHints.averageBitrate = 1200000;
        captureFromTexture.SetEncoderHints(hints);

        captureFromTexture.FrameRate = 30.0f;
        captureFromTexture.CameraRenderResolution = CaptureBase.Resolution.HD_1920x1080;

        captureFromTexture.OutputFolderPath = "Movie";
        captureFromTexture.AppendFilenameTimestamp = false;

        captureFromTexture.SetSourceTexture(movieRenderTexture);
        captureFromTexture.UnityAudioCapture = audioCapture;

        if (!captureFromTexture.IsCapturing())
        {
            captureFromTexture.FilenamePrefix = "HDRPRec";
            captureFromTexture.StartCapture();
        }
    }

    private void OnDestroy()
    {
        if (captureFromTexture.IsCapturing())
        {
            captureFromTexture.StopCapture();
        }
    }

    // Update is called once per frame
    void Update () {
        // Calculate the time it took to render the last frame
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Calculate frames per second
        fps = 1.0f / deltaTime;

    }

    private GUIStyle style;

    private void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.fontSize = 36;
        }
        var rc = new Rect(0, 0, 400, 62);
        GUI.Box(rc, texture);
        GUI.Label(rc, $"RECORDING. FPS: {fps:F2}", style);
        rc = new Rect(0, 70, 100, 40);
        if (GUI.Button(rc, "Do Something"))
        {
            Thread.Sleep(3000);
        }
    }
}
