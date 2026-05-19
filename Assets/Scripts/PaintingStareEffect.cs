using UnityEngine;

public class PaintingStareEffect : MonoBehaviour
{
    public Material normalMaterial;
    public Material creepyMaterial;
    public Renderer renderer;

    public float stareThreshold = 3.0f;
    public bool enableGlitch = true;

    private float stareTime = 0f;
    private bool isCreepy = false;

    private void Start()
    {
        if (renderer == null)
            renderer = GetComponent<Renderer>();

        if (normalMaterial != null)
            renderer.material = normalMaterial;
    }

    public void OnStare(float deltaTime)
    {
        stareTime += deltaTime;

        if (stareTime >= stareThreshold && !isCreepy)
        {
            isCreepy = true;
            if (creepyMaterial != null)
                renderer.material = creepyMaterial;

            Debug.Log(gameObject.name + " → CREEPY");
        }
    }

    public void OnLookAway()
    {
        stareTime = 0f;
        if (isCreepy)
        {
            isCreepy = false;
            if (normalMaterial != null)
                renderer.material = normalMaterial;
            Debug.Log(gameObject.name + " → Normal");
        }
    }
}
