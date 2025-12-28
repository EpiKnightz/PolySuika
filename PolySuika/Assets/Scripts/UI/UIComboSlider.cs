using PrimeTween;
using Sortify;
using UnityEngine;
using UnityEngine.UI;

public class UIComboSlider : MonoBehaviour
{
    public Slider ComboSlider;

    [BetterHeader("Listen To")]
    public FloatEventChannelSO ECOnComboTimeChange;

    private void Start()
    {
        EnableSlider(false);
    }

    private void Awake()
    {
        ECOnComboTimeChange.Sub(OnComboTimeChanges);
    }

    private void OnDestroy()
    {
        ECOnComboTimeChange.Unsub(OnComboTimeChanges);
    }

    public void OnComboTimeChanges(float Value)
    {
        if (Value > 0)
        {
            if (!gameObject.activeSelf)
            {
                EnableSlider(true);
            }
            ComboSlider.value = Value;
        }else if (gameObject.activeSelf) 
        {
            EnableSlider(false);
        }
    }

    void EnableSlider(bool enable)
    {
        gameObject.SetActive(enable);
    }
}
