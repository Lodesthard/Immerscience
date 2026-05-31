using UnityEngine;

public class ChairStareGlitched : MonoBehaviour
{
    [Header("Glitch Target Rotations")]
    [Tooltip("How much it tilts up on the X axis when stared at.")]
    public float upwardTiltAngle = -25f;
    [Tooltip("How much it twists horizontally on the Z axis when stared at.")]
    public float sidewardTwistAngle = 20f;
    public float speed = 5.0f;

    private Quaternion originalRotation;
    private Quaternion glitchRotation;
    private bool shouldGlitch = false;

    private void Start()
    {
        // Remember the normal room orientation
        originalRotation = transform.localRotation;

        // Precalculate what the glitched rotation looks like
        Vector3 glitchedEuler = originalRotation.eulerAngles + new Vector3(upwardTiltAngle, 0f, sidewardTwistAngle);
        glitchRotation = Quaternion.Euler(glitchedEuler);
    }

    private void Update()
    {
        // Smoothly move towards whichever rotation state is currently active
        Quaternion target = shouldGlitch ? glitchRotation : originalRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * speed);
    }

    // Call this from your Gaze Interactor system or Manager
    public void OnStare()
    {
        shouldGlitch = true;
    }

    // Overload in case an XR event automatically sends a float parameter
    public void OnStareFloat(float dummy)
    {
        shouldGlitch = true;
    }

    // The second the player looks away, it returns to normal
    public void OnLookAway()
    {
        shouldGlitch = false;
    }
}
