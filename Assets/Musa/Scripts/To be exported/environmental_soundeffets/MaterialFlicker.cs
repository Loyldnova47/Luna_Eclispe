using UnityEngine;

public class SecurityAlarmFlicker : MonoBehaviour
{
    [Header("Alarm Activation Trigger")]
    [Tooltip("Turn this ON to start the alarm. It will automatically turn off when the timer finishes!")]
    public bool isAlarmActive = false; 

    [Header("Auto Shut-off Settings")]
    [Tooltip("How many seconds the alarm runs before automatically turning itself off completely.")]
    public float totalAlarmDuration = 5.0f; 
    private float alarmShutOffTime;
    private bool alarmTimerStarted = false;

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
            alarmAudioSource.loop = false; 
        }

        ResetAlarmToNormal();
    }

    void Update()
    {
        if (targetMaterial == null) return;

        // 1. Handle Active Alarm States & Countdown
        if (isAlarmActive)
        {
            // FIX 1: Switched to Time.unscaledTime so pausing/unpausing doesn't freeze the calculations!
            if (!alarmTimerStarted)
            {
                alarmShutOffTime = Time.unscaledTime + totalAlarmDuration;
                alarmTimerStarted = true;
                Debug.Log($"<color=red>[Alarm System]</color> Alarm triggered! Will automatically shut down in {totalAlarmDuration} seconds.");
            }

            if (Time.unscaledTime >= alarmShutOffTime)
            {
                TriggerAlarmAlert(false); 
                return;
            }
        }
        else
        {
            if (isLightOn || isAudioPlaying || alarmTimerStarted)
            {
                ResetAlarmToNormal();
            }
            return;
        }

        // 2. Handle Light Flashing
        // FIX 2: Using unscaled time ticks ensures the blink cycle loops smoothly while playing
        if (Time.unscaledTime >= nextToggleTime)
        {
            isLightOn = !isLightOn;
            Color finalColor;

            if (isLightOn)
            {
                finalColor = alarmColor * Mathf.Pow(2f, onIntensity);
                nextToggleTime = Time.unscaledTime + lightTimeOn;

                if (alarmAudioSource != null)
                {
                    // Ensure the audio source is configured to play back even if global time freezes
                    alarmAudioSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
                    alarmAudioSource.Play();
                    isAudioPlaying = true;
                    stopAudioTime = Time.unscaledTime + audioDuration;
                }
            }
            else
            {
                finalColor = Color.black;
                nextToggleTime = Time.unscaledTime + lightTimeOff;
            }

            targetMaterial.SetColor(EmissionColorID, finalColor);
            DynamicGI.SetEmissive(objectRenderer, finalColor);
        }

        // 3. Handle Independent Audio Cut-off
        if (isAudioPlaying && Time.unscaledTime >= stopAudioTime)
        {
            if (alarmAudioSource != null)
            {
                alarmAudioSource.Stop();
            }
            isAudioPlaying = false;
        }
    }

    public void TriggerAlarmAlert(bool activate)
    {
        isAlarmActive = activate;
        if (!activate)
        {
            ResetAlarmToNormal();
        }
    }

    private void ResetAlarmToNormal()
    {
        isLightOn = false;
        isAudioPlaying = false;
        alarmTimerStarted = false;
        nextToggleTime = 0f;

        if (alarmAudioSource != null) alarmAudioSource.Stop();
        if (targetMaterial != null) targetMaterial.SetColor(EmissionColorID, Color.black);
        if (objectRenderer != null) DynamicGI.SetEmissive(objectRenderer, Color.black);
    }
}

