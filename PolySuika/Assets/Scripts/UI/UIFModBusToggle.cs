using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class UIFModBusToggle : UIToggle
{
    public Sprite EnableToggle;
    public Sprite DisableToggle;
    public Image TargetImage;

    public string busName;

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
        //RuntimeManager.MuteAllEvents(!enable);
        RuntimeManager.GetBus(busName).setMute(!enable);
    }

    
}
