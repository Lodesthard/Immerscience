using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Guitare : MonoBehaviour
{
    public Transform secondHandAttach;

    private XRGrabInteractable grab;
    private XRBaseInteractor secondInteractor;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    public void OnSecondHandGrab(XRBaseInteractor interactor)
    {
        secondInteractor = interactor;
    }

    public void OnSecondHandRelease()
    {
        secondInteractor = null;
    }

    void Update()
    {
        if (secondInteractor != null && grab.interactorsSelecting != null)
        {
            Transform firstHand = grab.interactorsSelecting[0].transform;
            Transform secondHand = secondInteractor.transform;

            Vector3 direction = secondHand.position - firstHand.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}