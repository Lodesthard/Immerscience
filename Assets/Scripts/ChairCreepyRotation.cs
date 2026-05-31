using UnityEngine;

public class ChairCreepyRotation : MonoBehaviour
{
    [Header("Time & Trigger States")]
    [Tooltip("How many seconds the player must be inside the Creepy Zone before the chair primes itself.")]
    public float zoneDelayThreshold = 5f;
    private float zoneTimer = 0f;
    private bool isInsideCreepyZone = false;
    private bool isPrimedAndReady = false;

    [Header("Rotation Settings")]
    [Tooltip("How far upwards it tilts on the X axis.")]
    public float upwardTiltAngle = -30f;
    [Tooltip("How far sidewards it twists on the Z axis.")]
    public float sidewardTwistAngle = 25f;
    [Tooltip("Speed of the smooth tilting interpolation.")]
    public float rotationSpeed = 4f;

    private Quaternion originalRotation;
    private Quaternion targetCreepyRotation;
    
    private bool isBeingStaredAt = false;
    private bool hallucinationTriggered = false;
    private bool hallucinationEndedForever = false;

    private void Start()
    {
        // Capture the exact starting position & angle from the room setup
        originalRotation = transform.localRotation;

        // Calculate the absolute twisted end-state target mathematically
        Vector3 targetEuler = originalRotation.eulerAngles + new Vector3(upwardTiltAngle, 0f, sidewardTwistAngle);
        targetCreepyRotation = Quaternion.Euler(targetEuler);
    }

    private void Update()
    {
        // 1. If the hallucination is done and put away, break out completely to optimize performance
        if (hallucinationEndedForever) return;

        // 2. Track the creepy zone time countdown
        if (isInsideCreepyZone && !isPrimedAndReady)
        {
            zoneTimer += Time.deltaTime;
            if (zoneTimer >= zoneDelayThreshold)
            {
                isPrimedAndReady = true;
                Debug.Log(gameObject.name + " is primed. Awaiting player eye contact...");
            }
        }

        // 3. Actively execute the creepy tilt only if they are staring and it's allowed
        if (isPrimedAndReady && isBeingStaredAt && !hallucinationEndedForever)
        {
            hallucinationTriggered = true;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetCreepyRotation, Time.deltaTime * rotationSpeed);
        }
        // 4. The absolute instant they break eye contact after triggering it, snap back and lock it down forever
        else if (hallucinationTriggered && !isBeingStaredAt)
        {
            transform.localRotation = originalRotation;
            hallucinationEndedForever = true;
            Debug.Log(gameObject.name + " hallucination terminated permanently.");
            
            // Self-delete this component so it literally can never compute calculation cycles again
            Destroy(this);
        }
    }

    // --- CALL THESE TWO FROM YOUR TRIGGER ZONE OVERLAP SCRIPT ---
    public void PlayerEnteredCreepyZone()
    {
        isInsideCreepyZone = true;
    }

    public void PlayerLeftCreepyZone()
    {
        isInsideCreepyZone = false;
        // If they leave the zone before 5 seconds are up, reset the countdown timer
        if (!isPrimedAndReady)
        {
            zoneTimer = 0f;
        }
    }

    // --- CALL THESE TWO FROM YOUR RAYCAST/STARE DETECTOR SCRIPT ---
    public void OnStare()
    {
        isBeingStaredAt = true;
    }

    public void OnLookAway()
    {
        isBeingStaredAt = false;
    }
}
