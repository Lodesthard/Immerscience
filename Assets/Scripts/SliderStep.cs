using UnityEngine;
using UnityEngine.UI;

public class SliderStep : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        float stepped = Mathf.Round(value * 2f) / 2f; // pas de 0.5
        slider.value = stepped;
    }
}