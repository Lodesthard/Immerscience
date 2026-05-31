using UnityEngine;

public class CreepyZoneManager : MonoBehaviour
{
    public static CreepyZoneManager Instance;
    public bool IsInsideCreepyZone = false;

    private void Awake()
    {
        Instance = this;
        Debug.Log("✅ CreepyZoneManager Ready");
    }

    private void Update()
    {
        if (Camera.main == null) return;

        float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
        Debug.Log($"📍 Distance to CreepyZone: {dist:F1} m");

        bool wasInside = IsInsideCreepyZone;
        IsInsideCreepyZone = (dist < 30f);   // Very big range

        if (IsInsideCreepyZone && !wasInside)
        {
            Debug.Log("✅✅✅ PLAYER ENTERED CREEPY ZONE!");
            if (AudioManager.Instance != null)
                AudioManager.Instance.StartMusic();
        }
    }
}
