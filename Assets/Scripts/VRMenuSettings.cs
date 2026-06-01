using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class VRMenuSettings : MonoBehaviour
{
    [Header("References")]
    public Transform xrOrigin;
    public ContinuousMoveProviderBase moveProvider;

    [Header("Sliders")]
    public Slider sliderSize;
    public Slider sliderSpeed;

    void Start()
    {
        if (sliderSize != null)
        {
            sliderSize.value = xrOrigin.localScale.x;
            // La taille ne s'applique qu'au RELÂCHEMENT du slider, pas pendant le drag.
            var rel = sliderSize.GetComponent<SliderApplyOnRelease>();
            if (rel == null) rel = sliderSize.gameObject.AddComponent<SliderApplyOnRelease>();
            rel.Released += OnSizeChanged;
        }

        if (sliderSpeed != null)
        {
            sliderSpeed.onValueChanged.AddListener(OnSpeedChanged);
            sliderSpeed.value = moveProvider.moveSpeed;
        }
    }

    void OnSizeChanged(float value)
    {
        xrOrigin.localScale = new Vector3(value, value, value);
    }

    void OnSpeedChanged(float value)
    {
        moveProvider.moveSpeed = value;
    }
}
