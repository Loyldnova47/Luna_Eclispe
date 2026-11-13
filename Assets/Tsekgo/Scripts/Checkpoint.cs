using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class Checkpoint : MonoBehaviour
{
    private RespawnScript respawn;
    private BoxCollider checkpointCollider;

    [Header("Light Glitch Settings")]
    public GameObject lightBeamObject; 
    public float glitchDuration = 1.5f; 
    public float minFlickerSpeed = 0.03f;
    public float maxFlickerSpeed = 0.12f;

    [Header("UI Message Settings")]
    public GameObject messageTextObject; 
    public TextMeshProUGUI messageText;
    public string messageString = "Checkpoint Reached!";
    public float messageDisplayTime = 3f; 

    void Awake()
    {
        checkpointCollider = GetComponent<BoxCollider>(); 
        respawn = GameObject.FindGameObjectWithTag("Respawn").GetComponent<RespawnScript>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            respawn.respawnPoint = this.gameObject;
            checkpointCollider.enabled = false; 

            // Start the light glitch and show the message
            StartCoroutine(GlitchAndDestroyLight());
            StartCoroutine(ShowCheckpointMessage());
        }
    }

    private IEnumerator GlitchAndDestroyLight()
    {
        if (lightBeamObject == null) yield break;

        float elapsed = 0f;

        
        while (elapsed < glitchDuration)
        {
            lightBeamObject.SetActive(!lightBeamObject.activeSelf);

            float waitTime = Random.Range(minFlickerSpeed, maxFlickerSpeed);
            yield return new WaitForSeconds(waitTime);
            elapsed += waitTime;
        }

        
        Destroy(lightBeamObject);
    }

    private IEnumerator ShowCheckpointMessage()
    {
        if (messageTextObject != null)
        {
            if (messageText != null)
            {
                messageText.text = messageString;
            }

            messageTextObject.SetActive(true);
            yield return new WaitForSeconds(messageDisplayTime);
            messageTextObject.SetActive(false);
        }
    }
}
