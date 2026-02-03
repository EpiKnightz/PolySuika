using FMODUnity;
using Reflex.Attributes;
using Sortify;
using UnityEngine;

public class UIFModBusToggle : UIToggle
{
    [Inject] private readonly IPref PrefManager;

    [Header("Variables")]
    [SerializeField] private string BusName;
    [SerializeField] private string SaveKey;

    private void OnEnable()
    {
        if (RuntimeManager.HaveAllBanksLoaded
            && RuntimeManager.GetBus(BusName).getMute(out bool mute) == FMOD.RESULT.OK)
        {
            if (mute == UIStateEnable)
            {
                OnClick();
            }
        }
    }

    public override void ToggleAction(bool enable)
    {
        PrefManager.SaveInt(SaveKey, enable ? 0 : 1);
        RuntimeManager.GetBus(BusName).setMute(!enable);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(SaveKey))
        {
            SaveKey = BusName[5..];
        }
    }
#endif
}
