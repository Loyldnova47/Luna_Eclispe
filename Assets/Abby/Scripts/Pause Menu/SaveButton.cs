using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    public Button button;
    public Color savedColor = new Color(1f, 0.7f, 0f, 1f); // gold orange

    public void Save()
    {
        GameObject respawnObj = GameObject.FindGameObjectWithTag("Respawn");
        RespawnScript respawn = respawnObj.GetComponent<RespawnScript>();

        if (respawn.respawnPoint == null)
        {
            Debug.Log("no checkpoint yet");
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        Vector3 pos = respawn.respawnPoint.transform.position;
        SaveSystem.Save(sceneName, pos);

        // make the button gold
        button.image.color = savedColor;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = Color.white;
        colors.selectedColor = Color.white;
        button.colors = colors;

        Debug.Log("saved");
    }
}