using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Demo scene: lets you change the AudioManager volume and shows that the value
// survives scene changes (go back to the menu and return).
public class DemoController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeLabel;
    [SerializeField] private Button backButton;
    [SerializeField, Tooltip("Build index of the main menu scene")]
    private int mainMenuSceneIndex = 1;

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("DemoController: no AudioManager found. Press Play from 00_Bootstrap.");
            volumeSlider.interactable = false;
        }
        else
        {
            // Show the volume the manager already holds (proves persistence).
            volumeSlider.value = AudioManager.Instance.Volume;
        }

        UpdateLabel(volumeSlider.value);

        // Subscribe after setting the starting value so it doesn't fire a change.
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        backButton.onClick.AddListener(LoadMainMenu);
    }

    private void OnVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolume(value);
        }
        UpdateLabel(value);
    }

    private void UpdateLabel(float value)
    {
        volumeLabel.text = "Music volume: " + Mathf.RoundToInt(value * 100f) + "%";
    }

    private void LoadMainMenu()
    {
        if (SaveManager.Instance != null) SaveManager.Instance.SaveSettings();

        if (SceneLoader.Instance == null)
        {
            Debug.LogWarning("DemoController: no SceneLoader found. Press Play from 00_Bootstrap.");
            return;
        }

        SceneLoader.Instance.LoadByIndex(mainMenuSceneIndex);
    }
}
