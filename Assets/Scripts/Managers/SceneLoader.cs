using UnityEngine;
using UnityEngine.SceneManagement;

// Persistent service that loads scenes by their build index.
// The indices come from the active Build Profile's scene list.
public class SceneLoader : MonoBehaviour
{
    // Global access point. Anyone can read it, only this class can set it.
    public static SceneLoader Instance { get; private set; }

    private void Awake()
    {
        // Duplicate guard: if another loader already exists, remove this one.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Loads the scene at the given build index.
    // Invalid indices log a warning instead of throwing an error.
    public void LoadByIndex(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("SceneLoader: build index " + buildIndex + " is not in the build list.");
            return;
        }

        SceneManager.LoadScene(buildIndex);
    }
}
