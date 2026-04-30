using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class LaserToggle : MonoBehaviour
{
    public InputActionReference toggleLasersAction;
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;

    private void OnEnable()
    {
        toggleLasersAction.action.performed += OnToggleLasers;
    }

    private void OnDisable()
    {
        toggleLasersAction.action.performed -= OnToggleLasers;
    }

    private void OnToggleLasers(InputAction.CallbackContext ctx)
    {
        bool newState = !leftRayInteractor.gameObject.activeSelf;
        leftRayInteractor.gameObject.SetActive(newState);
        rightRayInteractor.gameObject.SetActive(newState);
    }
}
