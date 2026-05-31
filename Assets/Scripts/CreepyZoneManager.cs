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

        // 1. Player just crossed into the zone
        if (IsInsideCreepyZone && !wasInside)
        {
            Debug.Log("✅✅✅ PLAYER ENTERED CREEPY ZONE!");
            if (AudioManager.Instance != null)
                AudioManager.Instance.StartMusic();

            // Find every chair in the scene with the script and tell them the countdown started
            ChairCreepyRotation[] chairs = FindObjectsByType<ChairCreepyRotation>(FindObjectsSortMode.None);
            foreach (ChairCreepyRotation chair in chairs)
            {
                chair.PlayerEnteredCreepyZone();
            }
        }
        
        // 2. Player just walked out of the zone
        else if (!IsInsideCreepyZone && wasInside)
        {
            Debug.Log("❌ PLAYER LEFT CREEPY ZONE!");
            
            // Tell all the chairs to reset their timers if they haven't triggered yet
            ChairCreepyRotation[] chairs = FindObjectsByType<ChairCreepyRotation>(FindObjectsSortMode.None);
            foreach (ChairCreepyRotation chair in chairs)
            {
                chair.PlayerLeftCreepyZone();
            }
        }
    }
}
