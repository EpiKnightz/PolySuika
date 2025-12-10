using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class UIComboSlider : MonoBehaviour
{
    public Slider ComboSlider;
    public Image[] SliderImages;

    private void Start()
    {
        EnableImages(false);
    }

    public void OnComboTimeChanges(float Value)
    {
        if (Value > 0)
        {
            if (SliderImages[0].enabled == false)
            {
                StartCombo();
            }
            ComboSlider.value = Value;
        }else if (SliderImages[0].enabled == true) 
        {
            HideComboSlider();
        }
    }

    void StartCombo()
    {
        EnableImages(true);
    }

    void HideComboSlider()
    {
        EnableImages(false);
    }

    void EnableImages(bool enable)
    {
        for (int i = 0; i < SliderImages.Length; i++)
        {
            SliderImages[i].enabled = enable;
        }
    }
}
