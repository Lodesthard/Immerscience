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
            sliderSize.onValueChanged.AddListener(OnSizeChanged);
            sliderSize.value = xrOrigin.localScale.x;
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
