using System;
using UnityEngine;
using UnityEngine.Events;
<<<<<<< Updated upstream
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
=======

public class Trigger : MonoBehaviour
{
    [SerializeField] bool destroyOnTriggerEnter;
    [SerializeField] string tagFilter;
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter (Collider other)
    {
        
        if (!String.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        onTriggerEnter.Invoke();
        if (destroyOnTriggerEnter)
        {
            Destroy (gameObject);
        }
    }

    // Update is called once per frame
    void OnTriggerExit (Collider other)
    {
        if (!String.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        onTriggerExit.Invoke();
    }
}
>>>>>>> Stashed changes
