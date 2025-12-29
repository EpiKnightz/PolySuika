using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using Sortify;

public class UIFModBusToggle : UIToggle
{
    [Header("References")]
    [SerializeField] private Sprite EnableToggle;
    [SerializeField] private Sprite DisableToggle;
    [SerializeField] private Image TargetImage;

    [BetterHeader("Variables")]
    [SerializeField] private string BusName;

    public override void DisableVisual()
    {
        if (TargetImage != null
            && DisableToggle != null)
        {
            TargetImage.sprite = DisableToggle;
        }
    }

    public override void EnableVisual()
    {
        if (TargetImage != null
            && EnableToggle != null)
        {
            TargetImage.sprite = EnableToggle;
        }
    }

    public override void ToggleAction(bool enable) 
    {
        RuntimeManager.GetBus(BusName).setMute(!enable);
    }
}
