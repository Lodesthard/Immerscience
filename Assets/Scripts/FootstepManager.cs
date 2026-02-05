using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    public AudioSource stepSource;
    public Transform playerCamera; // On va y glisser la Main Camera
    public float sensitivity = 0.01f; // Seuil de détection
    private Vector3 lastPosition;

    void Start()
    {
        if (playerCamera != null)
            lastPosition = playerCamera.position;
    }

    void Update()
    {
        if (stepSource == null || playerCamera == null) return;

        // On calcule la distance parcourue par la caméra depuis la frame précédente
        float distanceMoved = Vector3.Distance(playerCamera.position, lastPosition);

        if (distanceMoved > sensitivity)
        {
            if (!stepSource.isPlaying)
            {
                stepSource.Play();
            }
        }
        else
        {
            if (stepSource.isPlaying)
            {
                stepSource.Pause();
            }
        }

        lastPosition = playerCamera.position;
    }
}
