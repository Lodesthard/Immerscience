using UnityEngine;

public class CreepyZoneManager : MonoBehaviour
{
    public static CreepyZoneManager Instance;
    public bool IsInsideCreepyZone = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Camera.main == null) return;

        float dist = Vector3.Distance(transform.position, Camera.main.transform.position);

        IsInsideCreepyZone = (dist < 18f);
    }
}
