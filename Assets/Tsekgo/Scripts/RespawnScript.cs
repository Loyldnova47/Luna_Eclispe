using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    public GameObject defaultSpawnPoint;
    public GameObject respawnPoint;

    void Awake()
    {   //player respawns from their default position 
        if (respawnPoint == null)
        {
            respawnPoint = defaultSpawnPoint;
        }
    }
}