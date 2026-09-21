using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    public GameObject defaultSpawnPoint;
    public GameObject respawnPoint;

    void Awake()
    {
        if (respawnPoint == null)
        {
            respawnPoint = defaultSpawnPoint;
        }
    }
}