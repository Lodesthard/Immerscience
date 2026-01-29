using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MagnetReturn : MonoBehaviour
{
    public float snapDistance = 0.5f;
    public float returnSpeed = 5f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private XRGrabInteractable grab;
    private bool returning = false;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        grab = GetComponent<XRGrabInteractable>();

        grab.selectExited.AddListener(_ => TryReturn());
    }

    void TryReturn()
    {
        float d = Vector3.Distance(transform.position, initialPosition);
        if (d < snapDistance)
        {
            returning = true;
        }
    }

    void Update()
    {
        if (!returning) return;

        transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * returnSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, Time.deltaTime * returnSpeed);

        if (Vector3.Distance(transform.position, initialPosition) < 0.01f)
        {
            returning = false;
        }
    }
}
