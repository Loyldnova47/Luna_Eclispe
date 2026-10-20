using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem//Back up incase checkpoint system scrypt does'nt work.
{
    static bool loadingSave = false;
    static Vector3 savedPos;

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey("saveScene");
    }

    public static void Save(string sceneName, Vector3 pos)
    {
        PlayerPrefs.SetString("saveScene", sceneName);
        PlayerPrefs.SetFloat("saveX", pos.x);
        PlayerPrefs.SetFloat("saveY", pos.y);
        PlayerPrefs.SetFloat("saveZ", pos.z);
        PlayerPrefs.Save();
    }

    public static void Continue()
    {
        if (!HasSave())
        {
            return;
        }

        float x = PlayerPrefs.GetFloat("saveX");
        float y = PlayerPrefs.GetFloat("saveY");
        float z = PlayerPrefs.GetFloat("saveZ");
        savedPos = new Vector3(x, y, z);
        loadingSave = true;

        Time.timeScale = 1f;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(PlayerPrefs.GetString("saveScene"));
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey("saveScene");
        PlayerPrefs.DeleteKey("saveX");
        PlayerPrefs.DeleteKey("saveY");
        PlayerPrefs.DeleteKey("saveZ");
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (!loadingSave)
        {
            return;
        }
        loadingSave = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        CharacterController cc = player.GetComponent<CharacterController>(); // Ensuring player position is picked up
        if (cc != null)
        {
            cc.enabled = false;
        }

        player.transform.position = savedPos;

        if (cc != null)
        {
            cc.enabled = true;
        }

        GameObject respawnObj = GameObject.FindGameObjectWithTag("Respawn");// ensures that the new position is the new respawn point,
        if (respawnObj != null)
        {
            RespawnScript respawn = respawnObj.GetComponent<RespawnScript>();
            GameObject point = new GameObject("LoadedCheckpoint");
            point.transform.position = savedPos;
            respawn.respawnPoint = point;
        }
    }
}