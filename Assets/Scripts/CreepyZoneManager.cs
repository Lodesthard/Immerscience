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
            ChairStareGlitched[] chairs = FindObjectsByType<ChairStareGlitched>(FindObjectsSortMode.None);
            foreach (ChairStareGlitched chair in chairs)
            {
                chair.OnStare();
            }
        }
        
        // 2. Player just walked out of the zone
        else if (!IsInsideCreepyZone && wasInside)
        {
            Debug.Log("❌ PLAYER LEFT CREEPY ZONE!");

            // Coupe la musique de la creepy zone en quittant la zone (ex. quand
            // l'ascenseur téléporte le joueur dans la salle 3, à ~197 m d'ici).
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopMusic();

            // Tell all the chairs to reset their timers if they haven't triggered yet
            ChairStareGlitched[] chairs = FindObjectsByType<ChairStareGlitched>(FindObjectsSortMode.None);
            foreach (ChairStareGlitched chair in chairs)
            {
                chair.OnLookAway();
            }
        }
    }
}
