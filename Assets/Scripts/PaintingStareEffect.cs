using UnityEngine;

public class PaintingStareEffect : MonoBehaviour
{
    [Header("Materials")]
    public Material normalMaterial;
    public Material creepyMaterial;

    [Header("Settings")]
    public float stareThreshold = 3.0f;      // Time before effect starts
    public float meltSpeed = 0.45f;          // Speed of melting (lower = slower)
    public bool enableGlitch = true;         // Random quick flashes
    public bool enableFrameDissolve = false; // Only activate on some paintings

    private Renderer rend;
    private float stareTime = 0f;
    private float meltAmount = 0f;
    private bool isCreepy = false;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        if (normalMaterial != null)
            rend.material = normalMaterial;
    }

    public void OnStare(float deltaTime)
    {
        stareTime += deltaTime;

        if (stareTime >= stareThreshold)
        {
            meltAmount = Mathf.MoveTowards(meltAmount, 1f, meltSpeed * deltaTime);

            // Sync music distortion with melting
            if (AudioManager.Instance != null)
                AudioManager.Instance.UpdateDistortion(meltAmount);

            // Random Glitch effect
            if (enableGlitch && meltAmount > 0.3f && Random.value < 0.13f)
            {
                rend.material = creepyMaterial;
                Invoke("RevertMaterial", Random.Range(0.08f, 0.25f));
            }
        }
    }

    private void RevertMaterial()
    {
        if (meltAmount < 0.92f)
            rend.material = normalMaterial;
    }

    public void OnLookAway()
    {
        stareTime = 0f;
        meltAmount = Mathf.MoveTowards(meltAmount, 0f, meltSpeed * 1.8f * Time.deltaTime);

        if (AudioManager.Instance != null)
            AudioManager.Instance.UpdateDistortion(meltAmount);

        if (meltAmount < 0.1f)
        {
            isCreepy = false;
            rend.material = normalMaterial;
        }
    }

    private void Update()
    {
        // Full creepy when almost melted
        if (meltAmount > 0.9f)
        {
            rend.material = creepyMaterial;
            isCreepy = true;
        }

        // Frame Dissolve Effect (for selected paintings)
        if (enableFrameDissolve && rend.material.HasProperty("_Cutoff"))
        {
            rend.material.SetFloat("_Cutoff", meltAmount * 0.8f);
        }
    }
}
