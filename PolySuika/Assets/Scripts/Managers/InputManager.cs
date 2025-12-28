using Lean.Touch;
using Sortify;
using UnityEngine;

[RequireComponent(typeof(LeanFingerTap))]
public class InputManager : MonoBehaviour
{
    public LeanFingerTap LeanTapScript;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECHomeButtonTriggered;
    public VoidEventChannelSO ECOnActionAnimFinished;

    void Start()
    {
        OnEnableGameplay(false);
    }

    private void OnEnable()
    {
        ECOnActionAnimFinished.Sub(OnActionPhaseStart);
        ECHomeButtonTriggered.Sub(OnActionPhaseEnd);
    }

    private void OnDisable()
    {
        ECOnActionAnimFinished.UnSub(OnActionPhaseStart);
        ECHomeButtonTriggered.UnSub(OnActionPhaseEnd);
    }

    void OnEnableGameplay(bool bIsEnable)
    {
        if (LeanTapScript != null)
        {
            LeanTapScript.enabled = bIsEnable;
        }
    }

    void OnActionPhaseStart()
    {
        OnEnableGameplay(true);
        // Additional input enabling can be added here
    }

    void OnActionPhaseEnd()
    {
        OnEnableGameplay(false);
    }
}
