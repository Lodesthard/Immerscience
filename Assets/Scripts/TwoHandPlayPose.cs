using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class TwoHandPlayPose : MonoBehaviour
{
    [Header("Grip Points (sous-objets vides)")]
    [Tooltip("Point où la main gauche se positionne. Créer un GameObject enfant vide et le glisser ici.")]
    public Transform leftGripPoint;

    [Tooltip("Point où la main droite se positionne. Créer un GameObject enfant vide et le glisser ici.")]
    public Transform rightGripPoint;

    [Header("Snap")]
    [Tooltip("Vitesse de snap vers la pose. Plus haut = plus rapide.")]
    public float snapSpeed = 15f;

    [Header("Audio")]
    [Tooltip("Si vide, AudioSource ajouté automatiquement.")]
    public AudioSource audioSource;

    [Tooltip("Son joué quand l'objet est saisi à 2 mains.")]
    public AudioClip twoHandGrabSound;

    [Range(0f, 1f)] public float volume = 1f;

    [Tooltip("Désactive la physique (kinematic) pendant la tenue à 2 mains.")]
    public bool freezePhysicsWhileHeld = true;

    private XRGrabInteractable grab;
    private Rigidbody rb;
    private bool twoHandActive;
    private bool prevIsKinematic;

    // Positions monde cibles recalculées à chaque LateUpdate
    private Vector3 targetPosition;
    private Quaternion targetRotation;

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
        audioSource.PlayOneShot(twoHandGrabSound, volume);
    }

    void LateUpdate()
    {
        if (!twoHandActive) return;
        if (grab.interactorsSelecting.Count < 2) return;
        if (leftGripPoint == null || rightGripPoint == null) return;

        var interactors = grab.interactorsSelecting;

        // Détermine quelle main est gauche / droite selon le tag ou le nom
        Transform handA = (interactors[0] as MonoBehaviour)?.transform;
        Transform handB = (interactors[1] as MonoBehaviour)?.transform;

        if (handA == null || handB == null) return;

        Transform leftHand, rightHand;
        if (IsLeftHand(interactors[0]))
        {
            leftHand = handA;
            rightHand = handB;
        }
        else
        {
            leftHand = handB;
            rightHand = handA;
        }

        // On veut : leftGripPoint monde == leftHand.position, rightGripPoint monde == rightHand.position
        // Axe entre les deux grip points (local) -> axe entre les deux mains (monde)
        Vector3 gripAxis = rightGripPoint.position - leftGripPoint.position;    // monde actuel
        Vector3 handAxis = rightHand.position - leftHand.position;             // monde cible

        // Rotation à appliquer à l'objet pour aligner gripAxis sur handAxis
        Quaternion alignRot = Quaternion.identity;
        if (gripAxis.sqrMagnitude > 0.0001f && handAxis.sqrMagnitude > 0.0001f)
            alignRot = Quaternion.FromToRotation(gripAxis.normalized, handAxis.normalized);

        // Après rotation, le centre des grips doit coïncider avec le centre des mains
        Vector3 gripCenter = (leftGripPoint.position + rightGripPoint.position) * 0.5f;
        Vector3 handCenter = (leftHand.position + rightHand.position) * 0.5f;
        Vector3 posOffset = handCenter - gripCenter;

        targetPosition = transform.position + posOffset;
        targetRotation = alignRot * transform.rotation;

        float t = Mathf.Clamp01(Time.deltaTime * snapSpeed);
        transform.position = Vector3.Lerp(transform.position, targetPosition, t);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
    }

    static bool IsLeftHand(IXRSelectInteractor interactor)
    {
        var mb = interactor as MonoBehaviour;
        if (mb == null) return false;
        string n = mb.gameObject.name.ToLower();
        if (n.Contains("left")) return true;
        if (n.Contains("right")) return false;
        // Fallback : tag
        return mb.CompareTag("LeftHand");
    }
}
