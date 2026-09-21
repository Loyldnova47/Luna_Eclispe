using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor; // This namespace is required for SceneAsset
#endif

public class MenuManager : MonoBehaviour
{
    [Header("Scene Configuration")]
    // If you prefer drag-and-drop, use SceneAsset (Editor only)
#if UNITY_EDITOR
    public SceneAsset sceneToLoad;
#endif

    // Fallback string for the actual build
    [SerializeField] private string sceneName;

    // This automatically sets the string name when you drag a scene into the inspector
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (sceneToLoad != null)
        {
            sceneName = sceneToLoad.name;
        }
#endif
    }

    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("No scene has been assigned to MenuManager!");
        }
    }
}

