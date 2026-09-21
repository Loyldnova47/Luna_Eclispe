using UnityEngine;

public class SecurityAlarmFlicker : MonoBehaviour
{
    [Header("Material References")]
    public Renderer objectRenderer;
    [ColorUsage(true, true)] public Color alarmColor = Color.red;

    [Header("Audio Reference")]
    public AudioSource alarmAudioSource;

    [Header("Light Timing")]
    [Tooltip("How long the light stays blindingly bright (seconds)")]
    public float lightTimeOn = 0.6f; 
    [Tooltip("How long the light stays completely dark (seconds)")]
    public float lightTimeOff = 0.4f; 

    [Header("Audio Timing")]
    [Tooltip("How long the beep lasts after the light turns on. Keep this shorter than Light Time On!")]
    public float audioDuration = 0.15f; 

    [Header("Brightness Control")]
    public float onIntensity = 4.0f; 

    private Material targetMaterial;
    private float nextToggleTime;
    private float stopAudioTime;
    private bool isLightOn = false;
    private bool isAudioPlaying = false;

    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        if (objectRenderer != null)
        {
            targetMaterial = objectRenderer.material; 
            targetMaterial.EnableKeyword("_EMISSION");
        }

        if (alarmAudioSource != null)
        {
            alarmAudioSource.playOnAwake = false;
            alarmAudioSource.loop = false; // Turn off looping so our script controls the cut-off
        }
    }

    void Update()
    {
        if (targetMaterial == null) return;

        // 1. Handle Light Flashing
        if (Time.time >= nextToggleTime)
        {
            isLightOn = !isLightOn;
            Color finalColor;

            if (isLightOn)
            {
                finalColor = alarmColor * Mathf.Pow(2f, onIntensity);
                nextToggleTime = Time.time + lightTimeOn;

                // Play the sound right when the light snaps ON
                if (alarmAudioSource != null)
                {
                    alarmAudioSource.Play();
                    isAudioPlaying = true;
                    // Calculate the exact timestamp when the audio should stop
                    stopAudioTime = Time.time + audioDuration;
                }
            }
            else
            {
                finalColor = Color.black;
                nextToggleTime = Time.time + lightTimeOff;
            }

            targetMaterial.SetColor(EmissionColorID, finalColor);
            DynamicGI.SetEmissive(objectRenderer, finalColor);
        }

        // 2. Handle Independent Audio Cut-off
        if (isAudioPlaying && Time.time >= stopAudioTime)
        {
            if (alarmAudioSource != null)
            {
                alarmAudioSource.Stop();
            }
            isAudioPlaying = false;
        }
    }
}
