using UnityEngine;
using UnityEngine.InputSystem;

public class MenuOpener : MonoBehaviour
{
    public InputActionReference openMenuAction; // ton action OpenMenu
    public GameObject menu;                     // ton canvas/menu
    public Transform head;                      // la Main Camera

    private void OnEnable()
    {
        openMenuAction.action.performed += OnOpenMenu;
    }

    private void OnDisable()
    {
        openMenuAction.action.performed -= OnOpenMenu;
    }

    private void OnOpenMenu(InputAction.CallbackContext ctx)
    {
        // Si le menu est fermé → on le place devant la tête
        if (!menu.activeSelf)
        {
            Vector3 forward = head.forward;
            forward.y = 0; // pour éviter de viser vers le haut ou le bas

            menu.transform.position = head.position + forward.normalized * 0.01f;
            menu.transform.rotation = Quaternion.LookRotation(forward);
        }

        // Toggle (ouvre/ferme)
        menu.SetActive(!menu.activeSelf);
    }
}