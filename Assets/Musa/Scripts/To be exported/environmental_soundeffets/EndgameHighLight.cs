using UnityEngine;

public class EndGameHighlight : MonoBehaviour
{
    public Transform visualMarker;

    public float pulseSpeed = 2f;
    public float minScale = 0.9f;
    public float maxScale = 1.1f;

    private Vector3 baseScale;

    void Start()
    {
        baseScale = visualMarker.localScale; // capture the artist-set scale once
    }

    void Update()
    {
        float pulse = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        visualMarker.localScale = baseScale * pulse; // scale relative to original, preserves proportions
    }
}