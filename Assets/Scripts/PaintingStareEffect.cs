using UnityEngine;

public class PaintingStareEffect : MonoBehaviour
{
    public Material normalMaterial;
    public Material creepyMaterial;
    public Renderer renderer;

    private float stareTime = 0f;
    private bool isCreepy = false;

    void Start()
    {
        if (renderer == null)
            renderer = GetComponent<Renderer>();

        if (normalMaterial != null)
            renderer.material = normalMaterial;
    }

    public void OnStare(float deltaTime)
    {
        stareTime += deltaTime;

        if (stareTime >= 2.0f && !isCreepy)   // 2 secondes seulement pour tester
        {
            isCreepy = true;
            if (creepyMaterial != null && renderer != null)
            {
                renderer.material = creepyMaterial;
                Debug.Log("🖼 CREEPY EFFECT ACTIVATED on " + gameObject.name);
            }
        }
    }

    public void OnLookAway()
    {
        stareTime = 0f;
        if (isCreepy)
        {
            isCreepy = false;
            if (normalMaterial != null && renderer != null)
                renderer.material = normalMaterial;
            Debug.Log("🖼 Returned to normal on " + gameObject.name);
        }
    }
}
