using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverFeedback : MonoBehaviour
{
    public float scaleMultiplier = 1.1f;
    public float speed = 8f;
    public GameObject auraObject;

    private Vector3 originalScale;
    private XRBaseInteractable interactable;
    private bool isHovered = false;

    void Start()
    {
        originalScale = transform.localScale;
        interactable = GetComponent<XRBaseInteractable>();

        interactable.hoverEntered.AddListener(_ => OnHover(true));
        interactable.hoverExited.AddListener(_ => OnHover(false));

        if (auraObject) auraObject.SetActive(false);
    }

    void Update()
    {
        Vector3 targetScale = isHovered ? originalScale * scaleMultiplier : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }

    void OnHover(bool state)
    {
        isHovered = state;
        if (auraObject) auraObject.SetActive(state);
    }
}

