using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Émet Released(value) quand l'utilisateur LÂCHE le slider (fin de drag ou pointer up),
// pas pendant le glissement. Sert à n'appliquer un effet qu'au relâchement.
[RequireComponent(typeof(Slider))]
public class SliderApplyOnRelease : MonoBehaviour, IPointerUpHandler, IEndDragHandler
{
    public event Action<float> Released;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void OnPointerUp(PointerEventData eventData) => Fire();
    public void OnEndDrag(PointerEventData eventData) => Fire();

    private void Fire()
    {
        if (slider != null)
            Released?.Invoke(slider.value);
    }
}
