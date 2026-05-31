using UnityEngine;

public class PaintingStareEffect : MonoBehaviour
{
    public Material normalMaterial;
    public Renderer renderer;

    public enum EffectType { CreepyDistortion, SinkThroughWall, DisappearGlitch }
    
    [Header("Choose ONE Effect for this Painting")]
    public EffectType activeEffect = EffectType.CreepyDistortion;

    [Header("Fast Glitch Settings")]
    [Tooltip("How fast the painting shrinks and glitches away.")]
    public float vanishDuration = 0.4f;
    [Tooltip("How violent the fast glitch vibration is.")]
    public float glitchIntensity = 0.25f;
    public float glitchShakeSpeed = 50f;
    
    private float currentVanishTimer = 0f;
    private bool isGlitchedOut = false;

    [Header("1. Creepy Distortion Settings")]
    public Material creepyMaterial;
    public float stareThreshold = 3.0f;
    private float stareTime = 0f;
    private bool isCreepy = false;

    [Header("2. Sink THROUGH Wall Settings")]
    [Tooltip("Distance it deep-dives straight BACKWARDS through its own wall plane.")]
    public float sinkDistance = 2.0f; 
    public float sinkSpeed = 8.0f;

    private Vector3 originalPosition;
    private Vector3 currentTargetPosition;
    private Vector3 originalScale;

    private void Start()
    {
        if (renderer == null)
            renderer = GetComponent<Renderer>();

        if (normalMaterial != null && renderer != null)
            renderer.material = normalMaterial;

        // Cache native layout scales
        originalPosition = transform.position;
        currentTargetPosition = originalPosition;
        originalScale = transform.localScale;
    }

    private void Update()
    {
        // --- TYPE 1: SINK THROUGH WALL ---
        if (activeEffect == EffectType.SinkThroughWall)
        {
            transform.position = Vector3.Lerp(transform.position, currentTargetPosition, Time.deltaTime * sinkSpeed);
        }
        
        // --- TYPE 2: FAST GLITCH AND DISAPPEAR ---
        else if (activeEffect == EffectType.DisappearGlitch)
        {
            if (isGlitchedOut)
            {
                currentVanishTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(currentVanishTimer / vanishDuration);

                if (progress >= 1.0f)
                {
                    transform.localScale = Vector3.zero;
                    transform.position = originalPosition;
                }
                else
                {
                    transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);

                    // High-frequency tremor glitch
                    float shakeX = Mathf.Sin(Time.time * glitchShakeSpeed) * glitchIntensity * (1f - progress);
                    float shakeY = Mathf.Cos(Time.time * glitchShakeSpeed * 1.5f) * glitchIntensity * (1f - progress);
                    float shakeZ = Mathf.Sin(Time.time * glitchShakeSpeed * 2f) * glitchIntensity * (1f - progress);

                    transform.position = originalPosition + new Vector3(shakeX, shakeY, shakeZ);
                }
            }
        }
    }

    /// <summary>
    /// Processes whether the item falls within the corner of the player's eye or under a direct gaze.
    /// </summary>
    public void UpdatePeripheralGaze(Transform cameraTransform, float dotThreshold, bool isDirectRaycastHit)
    {
        // Vector heading directly from eye to object center
        Vector3 dirToPainting = (transform.position - cameraTransform.position).normalized;
        float lookAlignment = Vector3.Dot(cameraTransform.forward, dirToPainting);

        // --- CONDITION A: Looking right at it (or center vision alignment is met) ---
        if (isDirectRaycastHit || lookAlignment > dotThreshold)
        {
            // Shatter the illusion instantly! Reset positions immediately
            ResetIllusion();
        }
        // --- CONDITION B: It is sitting in the peripheral view (slightly turned away) ---
        else if (lookAlignment > 0.35f) // Adjust this lower if you want it to trigger even further to the side
        {
            switch (activeEffect)
            {
                case EffectType.SinkThroughWall:
                    // Uses local forward vector to sink straight BACKWARDS through its wall mesh
                    currentTargetPosition = originalPosition - (transform.forward * sinkDistance);
                    break;

                case EffectType.DisappearGlitch:
                    isGlitchedOut = true;
                    break;
            }
        }
        // --- CONDITION C: Completely behind the player's head ---
        else
        {
            ResetIllusion();
        }
    }

    private void ResetIllusion()
    {
        isGlitchedOut = false;
        currentVanishTimer = 0f;
        currentTargetPosition = originalPosition;

        // Snap parameters back instantly like it was never gone
        transform.position = originalPosition;
        transform.localScale = originalScale;
    }

    public void OnStare(float deltaTime)
    {
        // Standard code path untouched for CreepyDistortion timers
        if (activeEffect == EffectType.CreepyDistortion)
        {
            stareTime += deltaTime;
            if (stareTime >= stareThreshold && !isCreepy)
            {
                isCreepy = true;
                if (creepyMaterial != null && renderer != null) renderer.material = creepyMaterial;
                if (AudioManager.Instance != null) AudioManager.Instance.StartCreepyDistortion();
            }
        }
    }

    public void OnLookAway()
    {
        stareTime = 0f;
        if (activeEffect == EffectType.CreepyDistortion && isCreepy)
        {
            isCreepy = false;
            if (normalMaterial != null && renderer != null) renderer.material = normalMaterial;
            if (AudioManager.Instance != null) AudioManager.Instance.ReturnNormalMusic();
        }
    }
}
