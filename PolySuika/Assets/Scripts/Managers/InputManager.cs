using Lean.Touch;
using Sortify;
using UnityEngine;

[RequireComponent(typeof(LeanFingerTap))]
public class InputManager : MonoBehaviour
{
    public LeanFingerTap LeanTapScript;

    [BetterHeader("Broadcast On")]
    public VectorEventChannelSO ECOnClickPosition;

    [Header("Listen To")]
    public VoidEventChannelSO ECOnPauseInput;
    public VoidEventChannelSO ECOnUnpauseInput;
    public VoidEventChannelSO[] ECOnNeedDisableInput;
    public VoidEventChannelSO[] ECOnNeedEnableInput;

    // Privates
    private bool IsPause = true;

    private void OnEnable()
    {
        for (int i = 0; i < ECOnNeedEnableInput.Length; i++)
        {
            ECOnNeedEnableInput[i].Sub(OnActionPhaseStart);
        }
        for (int i = 0; i < ECOnNeedDisableInput.Length; i++)
        {
            ECOnNeedDisableInput[i].Sub(OnActionPhaseEnd);
        }
        ECOnPauseInput.Sub(OnPause);
        ECOnUnpauseInput.Sub(Unpause);
    }

    private void OnDisable()
    {
        for (int i = 0; i < ECOnNeedEnableInput.Length; i++)
        {
            ECOnNeedEnableInput[i].Unsub(OnActionPhaseStart);
        }
        for (int i = 0; i < ECOnNeedDisableInput.Length; i++)
        {
            ECOnNeedDisableInput[i].Unsub(OnActionPhaseEnd);
        }
        ECOnPauseInput.Unsub(OnPause);
        ECOnUnpauseInput.Unsub(Unpause);
    }

    public void OnPause()
    {
        IsPause = true;
    }

    public void Unpause()
    {
        IsPause = false;
    }

    public void OnClick(Vector3 position)
    {
        if (!IsPause)
        {
            ECOnClickPosition.Invoke(position);
        }
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
