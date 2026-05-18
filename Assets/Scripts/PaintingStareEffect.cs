using UnityEngine;

public class PaintingStareEffect : MonoBehaviour
{
    public Material normalMaterial;
    public Material creepyMaterial;
    public Renderer renderer;

    private float stareTime = 0f;
    private const float STARE_THRESHOLD = 3.5f;
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
        if (!CreepyZoneManager.Instance || !CreepyZoneManager.Instance.IsInsideCreepyZone)
        {
            if (isCreepy) DeactivateCreepy();
            stareTime = 0f;
            return;
        }

        stareTime += deltaTime;

        if (stareTime >= STARE_THRESHOLD && !isCreepy)
        {
            ActivateCreepy();
        }
    }

    public void OnLookAway()
    {
        stareTime = 0f;
        if (isCreepy)
        {
            DeactivateCreepy();
        }
    }

    public void ActivateCreepy()
    {
        isCreepy = true;
        if (creepyMaterial != null && renderer != null)
            renderer.material = creepyMaterial;

        if (AudioManager.Instance != null)
            AudioManager.Instance.StartCreepyDistortion();
    }

    public void DeactivateCreepy()
    {
        isCreepy = false;
        if (normalMaterial != null && renderer != null)
            renderer.material = normalMaterial;

        if (AudioManager.Instance != null)
            AudioManager.Instance.ReturnNormalMusic();
    }
}
