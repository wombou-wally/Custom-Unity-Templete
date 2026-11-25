using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Shows/hides the settings panel and keeps its controls in sync with SettingsManager.
// Every public method below is wired in the Inspector:
//   Settings button On Click        -> ShowPanel
//   Close button On Click           -> HidePanel
//   Dropdowns / toggles On Value Changed -> the matching Set... method (dynamic value)
// Dropdown options for Quality, Anti-aliasing and Frame rate cap are typed into the dropdowns.
// Only Resolution is filled here, because it depends on the player's monitor.
public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown antiAliasingDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown frameCapDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;

    private void Start()
    {
        panel.SetActive(false);

        if (SettingsManager.Instance == null)
        {
            Debug.LogWarning("SettingsPanel: no SettingsManager found. Press Play from 00_Bootstrap.");
            return;
        }

        FillResolutions();
        Refresh();
    }

    public void ShowPanel()
    {
        if (SettingsManager.Instance == null) return;
        Refresh();
        panel.SetActive(true);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
        if (SaveManager.Instance != null) SaveManager.Instance.SaveSettings();
    }

    public void SetQuality(int index)
    {
        SettingsManager.Instance.SetQualityLevel(index);
        Refresh();
    }

    public void SetAntiAliasing(int index)
    {
        SettingsManager.Instance.SetAntiAliasing(index);
        Refresh();
    }

    public void SetResolution(int index)
    {
        SettingsManager.Instance.SetResolution(index);
        Refresh();
    }

    public void SetFrameCap(int index)
    {
        SettingsManager.Instance.SetFrameRateCap(index);
        Refresh();
    }

    public void SetFullscreen(bool value)
    {
        SettingsManager.Instance.SetFullscreen(value);
        Refresh();
    }

    public void SetVSync(bool value)
    {
        SettingsManager.Instance.SetVSync(value);
        Refresh();
    }

    private void FillResolutions()
    {
        var labels = new List<string>();
        foreach (Resolution r in SettingsManager.Instance.Resolutions)
            labels.Add(r.width + " x " + r.height);
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(labels);
    }

    // Pushes manager values into the controls without firing their events.
    private void Refresh()
    {
        SettingsManager s = SettingsManager.Instance;
        qualityDropdown.SetValueWithoutNotify(s.QualityLevel);
        antiAliasingDropdown.SetValueWithoutNotify(s.AntiAliasingIndex);
        resolutionDropdown.SetValueWithoutNotify(s.ResolutionIndex);
        frameCapDropdown.SetValueWithoutNotify(s.FrameRateCapIndex);
        fullscreenToggle.SetIsOnWithoutNotify(s.Fullscreen);
        vsyncToggle.SetIsOnWithoutNotify(s.VSync);
    }
}
