using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void ClickQuitToMenu()
    {
        Debug.Log("Quitting to menu");
        SceneManager.LoadScene("Menu"); 
    }
}

