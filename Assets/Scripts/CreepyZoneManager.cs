using UnityEngine;

public class CreepyZoneManager : MonoBehaviour
{
    public static CreepyZoneManager Instance;

    public bool IsInsideCreepyZone { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            IsInsideCreepyZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            IsInsideCreepyZone = false;
    }
}
