using UnityEngine;

public class PaintingStareEffect : MonoBehaviour
{
    public Material normalMaterial;
    public Material creepyMaterial;
    public Renderer renderer;

    [Header("Settings")]
    public float stareThreshold = 3.0f;
    public float meltSpeed = 0.4f;
    public bool enableGlitch = true;

    private float stareTime = 0f;
    private float meltAmount = 0f;

    private void Start()
    {
        if (renderer == null)
            renderer = GetComponent<Renderer>();

        renderer.material = normalMaterial;
    }

    public void OnStare(float deltaTime)
    {
        stareTime += deltaTime;

        if (stareTime >= stareThreshold)
        {
            meltAmount = Mathf.MoveTowards(meltAmount, 1f, meltSpeed * deltaTime);

            // Glitch effect
            if (enableGlitch && Random.value < 0.08f)
            {
                renderer.material = creepyMaterial;
                Invoke("RevertMaterial", Random.Range(0.08f, 0.25f));
            }
        }
    }

    private void RevertMaterial()
    {
        if (meltAmount < 0.95f)
            renderer.material = normalMaterial;
    }

    public void OnLookAway()
    {
        stareTime = 0f;
        meltAmount = 0f;
        renderer.material = normalMaterial;
    }

    // Simple version for now - full switch when fully melted
    private void Update()
    {
        if (meltAmount > 0.92f)
        {
            renderer.material = creepyMaterial;
        }
    }
}
