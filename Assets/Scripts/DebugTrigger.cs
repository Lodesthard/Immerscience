using UnityEngine;

public class DebugTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("🔴 OnTriggerEnter called by: " + other.gameObject.name);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            Debug.Log("🟢 Player is INSIDE CreepyZone");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("🔴 OnTriggerExit called by: " + other.gameObject.name);
    }
}
