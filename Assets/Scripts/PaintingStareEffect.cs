using UnityEngine;

public class PaintingStareEffect : MonoBehaviour
{
    public Material normalMaterial;
    public Material creepyMaterial;
    public Renderer renderer;

    public float stareThreshold = 3.0f;
    public float meltSpeed = 0.5f;

    private float stareTime = 0f;
    private float meltAmount = 0f;
    private bool isCreepy = false;

    private void Start()
    {
        if (renderer == null)
            renderer = GetComponent<Renderer>();

        renderer.material = normalMaterial;
        Debug.Log(gameObject.name + " initialized");
    }

    public void OnStare(float deltaTime)
    {
        stareTime += deltaTime;

        if (stareTime >= stareThreshold)
        {
            meltAmount = Mathf.MoveTowards(meltAmount, 1f, meltSpeed * deltaTime);

            if (meltAmount >= 0.98f && !isCreepy)
            {
                isCreepy = true;
                renderer.material = creepyMaterial;
                Debug.Log(gameObject.name + " → FULL CREEPY");
            }
        }
    }

    public void OnLookAway()
    {
        stareTime = 0f;
        meltAmount = 0f;

        if (isCreepy)
        {
            isCreepy = false;
            renderer.material = normalMaterial;
            Debug.Log(gameObject.name + " → Back to normal");
        }
    }
}
