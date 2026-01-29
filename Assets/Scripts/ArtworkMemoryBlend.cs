using UnityEngine;

public class ArtworkMemoryBlend : MonoBehaviour
{
    public Transform listener;   // camera
    public float maxDistance = 6f;
    public float fadeSpeed = 2f;
    public float maxVolume = 1f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (!listener)
            listener = Camera.main.transform;

        audioSource.volume = 0f;
        audioSource.Play();
    }

    void Update()
    {
        float distance = Vector3.Distance(listener.position, transform.position);

        float targetVolume = 0f;

        if (distance < maxDistance)
        {
            float t = 1f - (distance / maxDistance);
            targetVolume = t * maxVolume;
        }

        audioSource.volume = Mathf.Lerp(
            audioSource.volume,
            targetVolume,
            Time.deltaTime * fadeSpeed
        );
    }
}
