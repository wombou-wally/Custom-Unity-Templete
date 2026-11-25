using UnityEngine;
using UnityEngine.UI;

// Controls the Main Menu scene. Only knows how to open the demo scene.
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button demoButton;
    [SerializeField, Tooltip("Build index of the demo scene")]
    private int demoSceneIndex = 2;

    private void Start()
    {
        demoButton.onClick.AddListener(LoadDemo);
    }

    private void LoadDemo()
    {
        if (SceneLoader.Instance == null)
        {
            Debug.LogWarning("MainMenuController: no SceneLoader found. Press Play from 00_Bootstrap.");
            return;
        }

        SceneLoader.Instance.LoadByIndex(demoSceneIndex);
    }
}
