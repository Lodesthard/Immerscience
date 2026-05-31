using UnityEngine;

public class Etage2MusicTrigger : MonoBehaviour
{
    private bool hasTriggered = false; // 👈 Guard variable

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return; // 👈 Quit if already triggered!

        bool isPlayer = false;
        if (other.CompareTag("Player")) isPlayer = true;
        if (other.transform.root.CompareTag("Player")) isPlayer = true;
        if (other.transform.root.name.Contains("XR Origin")) isPlayer = true;
        if (other.transform.root.name.Contains("Rig")) isPlayer = true;
        if (other.gameObject.name.Contains("Interactor")) isPlayer = true;
        if (other.gameObject.name.Contains("Camera")) isPlayer = true;

        if (isPlayer)
        {
            hasTriggered = true; // 👈 Lock it down immediately
            Debug.Log("✅ PLAYER ENTERED TRIGGER ZONE!");

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StartMusic();
            }
        }
    }
}
