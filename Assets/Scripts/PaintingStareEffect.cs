using UnityEngine;

public class PaintingStareEffect : MonoBehaviour
{
    public Material normalMaterial;
    public Renderer renderer;

    public enum EffectType { CreepyDistortion, SinkIntoWall, HoverInAir }
    public enum PushDirection { WorldForward, WorldBackward, WorldLeft, WorldRight }
    
    [Header("Choose ONE Effect for this Painting")]
    public EffectType activeEffect = EffectType.CreepyDistortion;

    [Header("Direction Into The Open Room")]
    [Tooltip("Which direction points AWAY from the wall into the walking space?")]
    public PushDirection roomDirection = PushDirection.WorldForward;

    [Header("Hallucination Settings (New)")]
    [Tooltip("How many seconds the physical movement lasts before it snaps back automatically.")]
    public float hallucinationDuration = 2.5f;
    private float currentHallucinationTimer = 0f;
    private bool hallucinationEnded = false;
    private bool isBeingStaredAt = false;

    [Header("1. Creepy Distortion Settings")]
    public Material creepyMaterial;
    public float stareThreshold = 3.0f;
    private float stareTime = 0f;
    private bool isCreepy = false;

    [Header("2. Sinking Wall Settings")]
    [Tooltip("Distance it sinks back into the wall frame.")]
    public float sinkDistance = 1.5f; 
    public float sinkSpeed = 3.0f;

    [Header("3. Weightless Air Hover Settings")]
    [Tooltip("How many meters the painting pops out into the room space.")]
    public float detachForwardDistance = 0.6f; 
    public float transitionSpeed = 2.0f;
    [Tooltip("How wide/far the random floating movement area is.")]
    public float floatRandomRange = 0.15f; 
    [Tooltip("How slow and smooth the random drift is.")]
    public float floatDriftSpeed = 0.8f;

    private Vector3 originalPosition;
    private Vector3 currentTargetPosition;
    private float noiseSeedX;
    private float noiseSeedY;
    private float noiseSeedZ;

    private void Start()
    {
        if (renderer == null)
            renderer = GetComponent<Renderer>();

        if (normalMaterial != null && renderer != null)
            renderer.material = normalMaterial;

        // Save the home position
        originalPosition = transform.position;
        currentTargetPosition = originalPosition;

        // Generate unique random seeds for organic drifting variance
        noiseSeedX = Random.Range(0f, 100f);
        noiseSeedY = Random.Range(0f, 100f);
        noiseSeedZ = Random.Range(0f, 100f);
    }

    private void Update()
    {
        // --- TIMER FOR MOVING HALLUCINATIONS ---
        // Exclude CreepyDistortion so it is completely unaffected by the timer
        if (isBeingStaredAt && !hallucinationEnded && activeEffect != EffectType.CreepyDistortion)
        {
            currentHallucinationTimer += Time.deltaTime;
            if (currentHallucinationTimer >= hallucinationDuration)
            {
                currentTargetPosition = originalPosition;
                hallucinationEnded = true;
            }
        }

        // --- TYPE 2: SINK INTO WALL ---
        if (activeEffect == EffectType.SinkIntoWall)
        {
            transform.position = Vector3.Lerp(transform.position, currentTargetPosition, Time.deltaTime * sinkSpeed);
        }
        
        // --- TYPE 3: WEIGHTLESS FLOAT/DRIFT ---
        else if (activeEffect == EffectType.HoverInAir)
        {
            // 1. Linearly move between the wall position and the floating point in space
            transform.position = Vector3.Lerp(transform.position, currentTargetPosition, Time.deltaTime * transitionSpeed);

            // 2. If it has detached, layer the organic random 3D drift on top (only if timer hasn't expired)
            if (currentTargetPosition != originalPosition && !hallucinationEnded)
            {
                float timeFactor = Time.time * floatDriftSpeed;
                
                // Calculate smooth random offsets using Perlin Noise
                float offsetX = (Mathf.PerlinNoise(noiseSeedX + timeFactor, 0f) - 0.5f) * floatRandomRange;
                float offsetY = (Mathf.PerlinNoise(0f, noiseSeedY + timeFactor) - 0.5f) * floatRandomRange;
                float offsetZ = (Mathf.PerlinNoise(noiseSeedZ + timeFactor, noiseSeedZ) - 0.5f) * floatRandomRange;

                // Add the smooth drifting motion to the painting
                transform.position += new Vector3(offsetX, offsetY, offsetZ);
            }
        }
    }

    private Vector3 GetChosenWorldVector()
    {
        switch (roomDirection)
        {
            case PushDirection.WorldForward: return Vector3.forward;
            case PushDirection.WorldBackward: return Vector3.back;
            case PushDirection.WorldLeft: return Vector3.left;
            case PushDirection.WorldRight: return Vector3.right;
            default: return Vector3.forward;
        }
    }

    public void OnStare(float deltaTime)
    {
        isBeingStaredAt = true;
        Vector3 pushVector = GetChosenWorldVector();

        // If a moving hallucination already timed out during this gaze session, stop pushing it
        if (hallucinationEnded && activeEffect != EffectType.CreepyDistortion) return;

        stareTime += deltaTime;

        switch (activeEffect)
        {
            case EffectType.SinkIntoWall:
                currentTargetPosition = originalPosition - (pushVector * sinkDistance);
                break;

            case EffectType.HoverInAir:
                currentTargetPosition = originalPosition + (pushVector * detachForwardDistance);
                break;

            case EffectType.CreepyDistortion:
                if (stareTime >= stareThreshold && !isCreepy)
                {
                    isCreepy = true;
                    if (creepyMaterial != null && renderer != null)
                        renderer.material = creepyMaterial;

                    if (AudioManager.Instance != null)
                        AudioManager.Instance.StartCreepyDistortion();

                    Debug.Log(gameObject.name + " → CREEPY + DISTORTION");
                }
                break;
        }
    }

    public void OnLookAway()
    {
        stareTime = 0f;
        currentHallucinationTimer = 0f;
        isBeingStaredAt = false;
        hallucinationEnded = false;

        // Reset positions smoothly back onto the wall mount
        if (activeEffect == EffectType.SinkIntoWall || activeEffect == EffectType.HoverInAir)
        {
            currentTargetPosition = originalPosition;
        }

        // Reset visual/audio for distortion effect
        if (activeEffect == EffectType.CreepyDistortion && isCreepy)
        {
            isCreepy = false;
            if (normalMaterial != null && renderer != null)
                renderer.material = normalMaterial;

            if (AudioManager.Instance != null)
                AudioManager.Instance.ReturnNormalMusic();

            Debug.Log(gameObject.name + " → Normal");
        }
    }
}
