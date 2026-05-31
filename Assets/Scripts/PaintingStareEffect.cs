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

    [Header("Hallucination Timer (New)")]
    [Tooltip("How many seconds does the hallucination last before reality snaps it back?")]
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
    public float sinkDistance = 1.5f; 
    public float sinkSpeed = 3.0f;

    [Header("3. Weightless Air Hover Settings")]
    public float detachForwardDistance = 0.6f; 
    public float transitionSpeed = 2.0f;
    public float floatRandomRange = 0.15f; 
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

        originalPosition = transform.position;
        currentTargetPosition = originalPosition;

        noiseSeedX = Random.Range(0f, 100f);
        noiseSeedY = Random.Range(0f, 100f);
        noiseSeedZ = Random.Range(0f, 100f);
    }

    private void Update()
    {
        // --- TIMER LOGIC FOR HALLUCINATION ---
        if (isBeingStaredAt && !hallucinationEnded)
        {
            currentHallucinationTimer += Time.deltaTime;
            if (currentHallucinationTimer >= hallucinationDuration)
            {
                // Hallucination over! Force the target back to the wall base
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
            transform.position = Vector3.Lerp(transform.position, currentTargetPosition, Time.deltaTime * transitionSpeed);

            // Only apply random drift if the hallucination is active and currently away from the wall
            if (currentTargetPosition != originalPosition && !hallucinationEnded)
            {
                float timeFactor = Time.time * floatDriftSpeed;
                
                float offsetX = (Mathf.PerlinNoise(noiseSeedX + timeFactor, 0f) - 0.5f) * floatRandomRange;
                float offsetY = (Mathf.PerlinNoise(0f, noiseSeedY + timeFactor) - 0.5f) * floatRandomRange;
                float offsetZ = (Mathf.PerlinNoise(noiseSeedZ + timeFactor, noiseSeedZ) - 0.5f) * floatRandomRange;

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

        // If the hallucination already timed out on this stare session, ignore further push logic
        if (hallucinationEnded) return;

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
                }
                break;
        }
    }

    public void OnLookAway()
    {
        // Reset everything cleanly so it's ready to flash again on the next glance
        stareTime = 0f;
        currentHallucinationTimer = 0f;
        isBeingStaredAt = false;
        hallucinationEnded = false; 

        if (activeEffect == EffectType.SinkIntoWall || activeEffect == EffectType.HoverInAir)
        {
            currentTargetPosition = originalPosition;
        }

        if (activeEffect == EffectType.CreepyDistortion && isCreepy)
        {
            isCreepy = false;
            if (normalMaterial != null && renderer != null)
                renderer.material = normalMaterial;

            if (AudioManager.Instance != null)
                AudioManager.Instance.ReturnNormalMusic();
        }
    }
}
