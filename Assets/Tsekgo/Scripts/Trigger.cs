using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Trigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Luna")
        {
            Debug.Log("load");
                 SceneManager.LoadScene("Game Over");
        }
    }

}