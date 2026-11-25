using UnityEngine;

// Lives only in 00_Bootstrap. Its job: make sure the shared managers exist,
// then hand over to the Main Menu. It does not run any gameplay.
public class Bootstrapper : MonoBehaviour
{
    [SerializeField, Tooltip("Build index of the scene to load once initialization is done")]
    private int firstSceneIndex = 1;

    // Start runs after every Awake in the scene, so persistent managers
    // have already registered themselves by now.
    private void Start()
    {
        if (AudioManager.Instance == null || SceneLoader.Instance == null || SettingsManager.Instance == null || SaveManager.Instance == null)
        {
            Debug.LogError("Bootstrapper: a manager was not initialized. Check the Bootstrap scene.");
            return;
        }

        // Apply the player's saved settings (if a save file exists) before the first scene.
        SaveManager.Instance.LoadSettings();

        Debug.Log("Bootstrapper: managers ready, loading scene " + firstSceneIndex);
        SceneLoader.Instance.LoadByIndex(firstSceneIndex);
    }
}
