using PrimeTween;
using Sortify;
using UnityEngine;

public class OffsetManager : MonoBehaviour
{
    [Header("Variables")]
    public float OffsetSpeed = 0.0225f;
    public float ResetDuration = 0.35f;
    public float SpeedFactor = 1.2f;
    public float MinSpeedMulti = 0.1f;
    public float MaxSpeedMulti = 50f;
    public float SpeedPower = 5f;

    [BetterHeader("Broadcast On")]
    public FloatEventChannelSO ECOnOffsetWorldYUpdated;

    [BetterHeader("Listen To")]
    public FloatEventChannelSO ECOnTriggerOffsetWorldY;
    public VoidEventChannelSO ECOnRestartTriggered;

    // Privates

    private void OnEnable()
    {
        ECOnTriggerOffsetWorldY.Sub(SetWorldOffsetY);
        ECOnRestartTriggered.Sub(ResetWorldOffsetY);
    }

    private void OnDisable()
    {
        ECOnTriggerOffsetWorldY.Unsub(SetWorldOffsetY);
        ECOnRestartTriggered.Unsub(ResetWorldOffsetY);
    }

    public float GetWorldOffsetY()
    {
        return transform.position.y;
    }

    public void SetWorldOffsetY(float offsetY)
    {
        Tween.StopAll(transform);
        if (offsetY > 0)
        {
            //WorldOffsetY = offsetY;
            float worldOffsetY = transform.position.y + offsetY;
            float speedMulti = Mathf.Clamp(Mathf.Pow(SpeedFactor, Mathf.FloorToInt(offsetY * SpeedPower)), MinSpeedMulti, MaxSpeedMulti);
            float duration = offsetY / (OffsetSpeed * speedMulti);
            //Debug.Log("OffsetManager: Setting World Offset Y to " + offsetY + " over duration " + duration + " with speed factor " + speedMulti);
            Tween.PositionY(transform, worldOffsetY, duration, ease: Ease.InOutSine).OnUpdate(target: this, (target, tween) => target.InvokeOffsetEvent());
        }
    }

    private void InvokeOffsetEvent()
    {
        ECOnOffsetWorldYUpdated?.Invoke(GetWorldOffsetY());
    }

    private void ResetWorldOffsetY()
    {
        Tween.StopAll(transform);
        if (GetWorldOffsetY() > 0)
        {
            Tween.PositionY(transform, 0, ResetDuration, ease: Ease.OutSine);
            InvokeOffsetEvent();
        }
    }
}
