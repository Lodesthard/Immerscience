using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class GuitarMelody : MonoBehaviour
{
    [Header("Refs")]
    public AudioSource melodySource;

    [Header("Optional")]
    public bool restartWhenTwoHands = true;

    private XRGrabInteractable grab;
    private XRBaseInteractor firstHand;   // main qui tient l'objet via XRGrabInteractable
    private XRBaseInteractor secondHand;  // main sur le manche (second grab point)

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        if (melodySource == null)
            melodySource = GetComponentInChildren<AudioSource>();

        // 1ère main: on s'accroche aux events de grab "principal"
        grab.selectEntered.AddListener(OnFirstHandGrab);
        grab.selectExited.AddListener(OnFirstHandRelease);
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnFirstHandGrab);
        grab.selectExited.RemoveListener(OnFirstHandRelease);
    }

    private void OnFirstHandGrab(SelectEnterEventArgs args)
    {
        firstHand = args.interactorObject as XRBaseInteractor;
        TryStartOrStop();
    }

    private void OnFirstHandRelease(SelectExitEventArgs args)
    {
        firstHand = null;
        StopMelody();
    }

    // Appelé par le "grab point" du manche
    public void SetSecondHand(XRBaseInteractor interactor)
    {
        secondHand = interactor;
        TryStartOrStop();
    }

    // Appelé quand on retire la main du manche
    public void ClearSecondHand(XRBaseInteractor interactor)
    {
        if (secondHand == interactor)
            secondHand = null;

        StopMelody();
    }

    private void TryStartOrStop()
    {
        if (firstHand != null && secondHand != null)
        {
            StartMelody();
        }
        else
        {
            StopMelody();
        }
    }

    private void StartMelody()
    {
        if (melodySource == null) return;

        if (restartWhenTwoHands)
            melodySource.time = 0f;

        if (!melodySource.isPlaying)
            melodySource.Play();
    }

    private void StopMelody()
    {
        if (melodySource == null) return;

        if (melodySource.isPlaying)
            melodySource.Stop();
    }
}