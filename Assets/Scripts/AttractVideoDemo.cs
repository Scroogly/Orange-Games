using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class AttractVideoDemo : MonoBehaviour
{
    [Header("Setup")]
    public Canvas demoCanvas;       // Canvas that contains the RawImage + VideoPlayerGO
    public VideoPlayer videoPlayer; // The VideoPlayer component
    public string videoFileName = "attract.mp4"; // Your video in StreamingAssets

    [Header("Behavior")]
    public float idleSeconds = 10f; // time with no input before demo starts
    public bool pauseGameplay = true; // pause the game while video plays

    private float lastHumanInput;
    private bool inDemo;

    // Shortcut to get the Canvas GameObject
    GameObject DemoRoot => demoCanvas ? demoCanvas.gameObject : null;

    void Start()
    {
        lastHumanInput = Time.unscaledTime;

        // Hide the canvas at start (turn off the entire GameObject)
        if (DemoRoot) DemoRoot.SetActive(false);

        // Set up the VideoPlayer to read from StreamingAssets
        if (videoPlayer)
        {
            string path = Path.Combine(Application.streamingAssetsPath, videoFileName);
            videoPlayer.isLooping = true;
            videoPlayer.url = path;
        }
    }

    void Update()
    {
        if (HumanInputDetected())
        {
            lastHumanInput = Time.unscaledTime;
            if (inDemo) ExitDemo();
        }

        if (!inDemo && Time.unscaledTime - lastHumanInput >= idleSeconds)
            EnterDemo();
    }

    bool HumanInputDetected()
    {
        if (Input.anyKeyDown) return true;

        // Detect movement or mouse activity
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f) return true;
        if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f) return true;
        if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0.01f) return true;
        if (Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.01f) return true;
        if (Input.GetKeyDown(KeyCode.Space)) return true;

        return false;
    }

    void EnterDemo()
    {
        inDemo = true;

        // Enable Canvas and VideoPlayer
        if (DemoRoot) DemoRoot.SetActive(true);

        // Pause the gameplay if requested
        if (pauseGameplay) Time.timeScale = 0f;

        if (videoPlayer) videoPlayer.Play();
    }

    void ExitDemo()
    {
        inDemo = false;

        if (videoPlayer) videoPlayer.Stop();

        // Hide Canvas and resume game
        if (DemoRoot) DemoRoot.SetActive(false);
        if (pauseGameplay) Time.timeScale = 1f;
    }
}
