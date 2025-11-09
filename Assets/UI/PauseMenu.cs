using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    GameObject canvasGO, dimmerGO, panelGO, eventSystemGO;
    bool paused;

    void Start()
    {
        // --- EventSystem (required for UI clicks) ---
        if (FindObjectOfType<EventSystem>() == null)
        {
            eventSystemGO = new GameObject("EventSystem");
#if ENABLE_INPUT_SYSTEM
            eventSystemGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
#endif
        }

        // --- Canvas ---
        canvasGO = new GameObject("PauseCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // --- Dimmer (full screen) ---
        dimmerGO = new GameObject("Dimmer");
        dimmerGO.transform.SetParent(canvasGO.transform, false);
        var dimmerImg = dimmerGO.AddComponent<Image>();
        dimmerImg.color = new Color(0f, 0f, 0f, 0.55f);
        var dRect = dimmerGO.GetComponent<RectTransform>();
        dRect.anchorMin = Vector2.zero;
        dRect.anchorMax = Vector2.one;
        dRect.offsetMin = Vector2.zero;
        dRect.offsetMax = Vector2.zero;

        // --- Panel (centered) ---
        panelGO = new GameObject("PausePanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        var panelImg = panelGO.AddComponent<Image>();
        panelImg.color = new Color(0.85f, 0.85f, 0.85f, 1f);
        var pRect = panelGO.GetComponent<RectTransform>();
        pRect.sizeDelta = new Vector2(500, 400);
        pRect.anchorMin = pRect.anchorMax = new Vector2(0.5f, 0.5f);
        pRect.pivot = new Vector2(0.5f, 0.5f);
        pRect.anchoredPosition = Vector2.zero;

        var layout = panelGO.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 25;
        layout.padding = new RectOffset(30, 30, 30, 30);

        // --- Title + Buttons ---
        CreateLabel("PAUSED", 40, FontStyle.Bold);
        CreateButton("Resume", ResumeGame);
        CreateButton("Restart", RestartGame);
        CreateButton("Quit", QuitGame);

        SetVisible(false);
        paused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        paused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        SetVisible(true);
    }

    public void ResumeGame()
    {
        paused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SetVisible(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void SetVisible(bool v)
    {
        if (dimmerGO) dimmerGO.SetActive(v);
        if (panelGO) panelGO.SetActive(v);
    }

    void CreateLabel(string text, int size, FontStyle style)
    {
        var go = new GameObject("Title");
        go.transform.SetParent(panelGO.transform, false);
        var le = go.AddComponent<LayoutElement>();
        le.minHeight = 60;

        var t = go.AddComponent<Text>();
        t.text = text;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // Unity 6 built-in font
        t.fontSize = size;
        t.fontStyle = style;
        t.color = Color.black;
        t.alignment = TextAnchor.MiddleCenter;
    }

    void CreateButton(string label, UnityEngine.Events.UnityAction onClick)
    {
        var btnGO = new GameObject(label + "Button");
        btnGO.transform.SetParent(panelGO.transform, false);

        var le = btnGO.AddComponent<LayoutElement>();
        le.minWidth = 330; le.minHeight = 70;

        var img = btnGO.AddComponent<Image>();
        img.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        var btn = btnGO.AddComponent<Button>();
        btn.onClick.AddListener(onClick);

        var txtGO = new GameObject("Text");
        txtGO.transform.SetParent(btnGO.transform, false);
        var t = txtGO.AddComponent<Text>();
        t.text = label;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = 26;
        t.color = Color.black;
        t.alignment = TextAnchor.MiddleCenter;

        var tr = txtGO.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.zero;
    }
}
