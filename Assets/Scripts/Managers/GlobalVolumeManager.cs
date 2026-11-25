using UnityEngine;

// Keeps one Global Volume (the project's post-processing look) alive across scenes.
// Put this on the root Global Volume object in 00_Bootstrap.
public class GlobalVolumeManager : MonoBehaviour
{
    public static GlobalVolumeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
