using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class TwoHandPlayPose : MonoBehaviour
{
    [Header("Cible (tête du joueur)")]
    [Tooltip("Glisser la Main Camera (tête XR). Si vide, le script tente de la retrouver via Camera.main.")]
    public Transform playerHead;

    [Header("Pose de jeu (relative à la tête)")]
    [Tooltip("Décalage local en mètres par rapport à la caméra du joueur.")]
    public Vector3 localPositionOffset = new Vector3(0.2f, -0.3f, 0.4f);

    [Tooltip("Rotation locale (en degrés) par rapport à la caméra du joueur.")]
    public Vector3 localEulerRotation = Vector3.zero;

    [Tooltip("Vitesse de snap vers la pose de jeu. Plus haut = plus rapide.")]
    public float snapSpeed = 12f;

    [Header("Audio (déclenché à la prise à 2 mains)")]
    [Tooltip("Si vide, un AudioSource sera ajouté automatiquement.")]
    public AudioSource audioSource;

    [Tooltip("Le son à jouer quand l'objet est saisi à 2 mains. Configurable dans l'inspecteur.")]
    public AudioClip twoHandGrabSound;

    [Range(0f, 1f)] public float volume = 1f;

    [Tooltip("Désactive la physique (rigidbody kinematic) tant que l'objet est tenu à 2 mains.")]
    public bool freezePhysicsWhileHeld = true;

    private XRGrabInteractable grab;
    private Rigidbody rb;
    private bool twoHandActive;
    private bool prevIsKinematic;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }

        if (playerHead == null && Camera.main != null)
            playerHead = Camera.main.transform;

        grab.selectEntered.AddListener(OnSelectEntered);
        grab.selectExited.AddListener(OnSelectExited);
    }

    void OnDestroy()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnSelectEntered);
            grab.selectExited.RemoveListener(OnSelectExited);
        }
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (grab.interactorsSelecting.Count >= 2 && !twoHandActive)
        {
            twoHandActive = true;
            PlayGrabSound();

            if (freezePhysicsWhileHeld && rb != null)
            {
                prevIsKinematic = rb.isKinematic;
                rb.isKinematic = true;
            }
        }
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        if (grab.interactorsSelecting.Count < 2 && twoHandActive)
        {
            twoHandActive = false;

            if (freezePhysicsWhileHeld && rb != null)
                rb.isKinematic = prevIsKinematic;
        }
    }

    void PlayGrabSound()
    {
        if (twoHandGrabSound == null) return;

        if (audioSource != null)
            audioSource.PlayOneShot(twoHandGrabSound, volume);
        else
            AudioSource.PlayClipAtPoint(twoHandGrabSound, transform.position, volume);
    }

    void LateUpdate()
    {
        if (!twoHandActive || playerHead == null) return;

        Vector3 targetPos = playerHead.TransformPoint(localPositionOffset);
        Quaternion targetRot = playerHead.rotation * Quaternion.Euler(localEulerRotation);

        float t = Mathf.Clamp01(Time.deltaTime * snapSpeed);
        transform.position = Vector3.Lerp(transform.position, targetPos, t);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, t);
    }
}
