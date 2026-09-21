using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.UI;

public class Floating_HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Camera camera;
    [SerializeField] private Transform target;
     
   public void UpdateHealthBar(float currentValue, float maxValue)
    {
        if (slider != null && maxValue > 0)
        {
            // FIX: Divides current by max to create a perfect fraction between 0.0 and 1.0
            slider.value = currentValue / maxValue;
        }
    }

    // FIX: Add a Billboard tracker to force the health bar to ALWAYS look flat at your Camera view
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            // This stops the canvas from flipping or rotating when the enemy turns around!
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera.transform.rotation;
        transform.position = target.position;
    }
}
