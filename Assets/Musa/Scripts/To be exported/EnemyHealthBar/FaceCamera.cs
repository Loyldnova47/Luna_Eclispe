using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform camTransform;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (camTransform != null)
        {
           // Forces the UI to match the camera's exact rotation direction
           transform.rotation = camTransform.rotation;
        }
    }
}
