using UnityEngine;

public class Etage2MusicTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called by: " + other.gameObject.name);

        // Aggressive player detection
        if (other.CompareTag("Player") || 
            other.transform.root.CompareTag("Player") || 
            other.transform.root.name.Contains("XR Origin") ||
            other.transform.root.name.Contains("Rig") ||
            other.gameObject.name.Contains("Interactor"))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StartMusic();
                Debug.Log("🎵✅✅✅ MUSIC FORCED STARTED!");
            }
            else
            {
                Debug.LogError("AudioManager.Instance is NULL!");
            }
        }
    }
}