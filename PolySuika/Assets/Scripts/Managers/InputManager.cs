using Lean.Touch;
using Sortify;
using UnityEngine;

[RequireComponent(typeof(LeanFingerTap))]
public class InputManager : MonoBehaviour
{
    public LeanFingerTap LeanTapScript;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECActionHidden;
    public VoidEventChannelSO ECOnActionAnimFinished;

    private void Start()
    {
        OnEnableGameplay(false);
    }

    private void OnEnable()
    {
        ECOnActionAnimFinished.Sub(OnActionPhaseStart);
        ECActionHidden.Sub(OnActionPhaseEnd);
    }

    private void OnDisable()
    {
        ECOnActionAnimFinished.Unsub(OnActionPhaseStart);
        ECActionHidden.Unsub(OnActionPhaseEnd);
    }

    private void OnEnableGameplay(bool bIsEnable)
    {
        if (LeanTapScript != null)
        {
            LeanTapScript.enabled = bIsEnable;
        }
    }

    private void OnActionPhaseStart()
    {
        OnEnableGameplay(true);
        // Additional input enabling can be added here
    }

    private void OnActionPhaseEnd()
    {
        OnEnableGameplay(false);
    }
}
