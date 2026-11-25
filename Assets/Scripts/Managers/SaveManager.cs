using System.IO;
using UnityEngine;

// Plain data that goes into the JSON file. No references to other objects.
[System.Serializable]
public class SaveData
{
    public float musicVolume = 1f;
    public int quality = 1;
    public int antiAliasing = 0;
    public int resolutionWidth = 0;
    public int resolutionHeight = 0;
    public bool fullscreen = true;
    public bool vsync = true;
    public int frameRateCap = 0;
}

// Persistent service: saves and loads the player's settings as a JSON file.
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string filePath;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "Savedata.json");
        Debug.Log("SaveManager: save file path is " + filePath);
    }

    private void OnApplicationQuit()
    {
        if (Instance == this) SaveSettings();
    }

    // Reads the current values from the managers and writes them to disk.
    public void SaveSettings()
    {
        var data = new SaveData();

        if (AudioManager.Instance != null)
            data.musicVolume = AudioManager.Instance.Volume;

        SettingsManager s = SettingsManager.Instance;
        if (s != null)
        {
            data.quality = s.QualityLevel;
            data.antiAliasing = s.AntiAliasingIndex;
            data.fullscreen = s.Fullscreen;
            data.vsync = s.VSync;
            data.frameRateCap = s.FrameRateCapIndex;
            if (s.Resolutions.Count > 0)
            {
                data.resolutionWidth = s.Resolutions[s.ResolutionIndex].width;
                data.resolutionHeight = s.Resolutions[s.ResolutionIndex].height;
            }
        }

        Save(data);
    }

    // Loads the file (if any) and pushes the values into the managers.
    public void LoadSettings()
    {
        SaveData data = Load();
        if (data == null) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetVolume(data.musicVolume);
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.ApplySaved(data);
    }

    public void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    // Returns null when there is no file yet (or it cannot be read).
    public SaveData Load()
    {
        if (!File.Exists(filePath))
        {
            Debug.Log("SaveManager: no save file yet, using defaults.");
            return null;
        }

        try
        {
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(filePath));
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("SaveManager: could not read save file. " + e.Message);
            return null;
        }
    }
}
