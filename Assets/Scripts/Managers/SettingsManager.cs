using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

// Persistent service that applies player settings (graphics, display, frame rate).
// Every setting goes through one method so a save system can hook in later.
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    // Maps the frame-cap dropdown option (0..4) to a real frame rate. -1 = unlimited.
    // The option labels themselves are typed into the dropdown in the Inspector.
    private static readonly int[] FrameRateCaps = { -1, 30, 60, 120, 144 };

    [SerializeField, Tooltip("Quality level applied at startup (0 Low, 1 Medium, 2 High)")]
    private int defaultQualityLevel = 1;

    private readonly List<Resolution> resolutions = new List<Resolution>();

    public IReadOnlyList<Resolution> Resolutions
    {
        get { return resolutions; }
    }
    public int QualityLevel { get; private set; }
    public int AntiAliasingIndex { get; private set; }
    public int ResolutionIndex { get; private set; }
    public bool Fullscreen { get; private set; }
    public bool VSync { get; private set; }
    public int FrameRateCapIndex { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildResolutionList();
        Fullscreen = Screen.fullScreen;
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Defaults
        SetQualityLevel(defaultQualityLevel);
        SetVSync(true);
        SetFrameRateCap(0);
    }

    private void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Each scene has its own camera, so anti-aliasing is re-applied after every load.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyAntiAliasing();
    }

    public void SetQualityLevel(int level)
    {
        QualityLevel = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(QualityLevel, true);
        // Changing level resets vSyncCount, so apply our own value again.
        ApplyVSync();
        Application.targetFrameRate = FrameRateCaps[FrameRateCapIndex];
    }

    public void SetAntiAliasing(int index)
    {
        AntiAliasingIndex = Mathf.Clamp(index, 0, 2);
        ApplyAntiAliasing();
    }

    public void SetResolution(int index)
    {
        if (resolutions.Count == 0) return;
        ResolutionIndex = Mathf.Clamp(index, 0, resolutions.Count - 1);
        ApplyResolution();
    }

    public void SetFullscreen(bool fullscreen)
    {
        Fullscreen = fullscreen;
        ApplyResolution();
    }

    public void SetVSync(bool on)
    {
        VSync = on;
        ApplyVSync();
        if (on)
        {
            // VSync overrides the frame cap, so reset the cap to match.
            FrameRateCapIndex = 0;
            Application.targetFrameRate = -1;
        }
    }

    public void SetFrameRateCap(int index)
    {
        FrameRateCapIndex = Mathf.Clamp(index, 0, FrameRateCaps.Length - 1);
        int cap = FrameRateCaps[FrameRateCapIndex];
        if (cap > 0)
        {
            // A cap only works with VSync off.
            VSync = false;
            ApplyVSync();
        }
        Application.targetFrameRate = cap;
    }

    // Pushes loaded save data through the normal Set... methods.
    public void ApplySaved(SaveData data)
    {
        SetQualityLevel(data.quality);
        SetAntiAliasing(data.antiAliasing);

        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == data.resolutionWidth && resolutions[i].height == data.resolutionHeight)
            {
                SetResolution(i);
                break;
            }
        }

        SetFullscreen(data.fullscreen);
        SetVSync(data.vsync);
        SetFrameRateCap(data.frameRateCap);
    }

    private void ApplyVSync()
    {
        QualitySettings.vSyncCount = VSync ? 1 : 0;
    }

    private void ApplyResolution()
    {
        if (resolutions.Count == 0) return;
        Resolution r = resolutions[ResolutionIndex];
        Screen.SetResolution(r.width, r.height,
            Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
    }

    private void ApplyAntiAliasing()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        UniversalAdditionalCameraData data = cam.GetUniversalAdditionalCameraData();
        switch (AntiAliasingIndex)
        {
            case 1:
                data.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                break;
            case 2:
                data.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                data.antialiasingQuality = AntialiasingQuality.High;
                break;
            default:
                data.antialiasing = AntialiasingMode.None;
                break;
        }
    }

    // Unique width x height pairs, in the order Unity reports them.
    private void BuildResolutionList()
    {
        resolutions.Clear();
        foreach (Resolution r in Screen.resolutions)
        {
            bool exists = false;
            foreach (Resolution known in resolutions)
            {
                if (known.width == r.width && known.height == r.height) { exists = true; break; }
            }
            if (!exists) resolutions.Add(r);
        }

        ResolutionIndex = Mathf.Max(0, resolutions.Count - 1);
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                ResolutionIndex = i;
                break;
            }
        }
    }
}
