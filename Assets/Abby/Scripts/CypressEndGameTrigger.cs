using UnityEngine;
using UnityEngine.SceneManagement;

public class CypressEndGameTrigger: MonoBehaviour
{
    [SerializeField] string finalSceneName;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("player reached the end, loading final scene");
        SceneManager.LoadScene(finalSceneName);
    }
}
