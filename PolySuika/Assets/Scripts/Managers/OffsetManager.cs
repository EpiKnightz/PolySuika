using PrimeTween;
using Sortify;
using UnityEngine;

public class OffsetManager : MonoBehaviour
{
    [Header("Variables")]
    public float OffsetSpeed = 0.0225f;
    public float ResetDuration = 0.35f;
    public float SpeedFactor = 1.2f;
    public float SpeedPower = 5f;

    [BetterHeader("Listen To")]
    public FloatEventChannelSO ECOnTriggerOffsetWorldY;
    public VoidEventChannelSO ECOnRestartTriggered;

    // Privates
    private float WorldOffsetY = 0f;

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
        return WorldOffsetY;
    }

    public void SetWorldOffsetY(float offsetY)
    {
        WorldOffsetY = offsetY;
        Tween.StopAll(transform);
        float nextPoint = transform.position.y + offsetY;
        float speedMulti = Mathf.Pow(SpeedFactor, Mathf.FloorToInt(offsetY * SpeedPower));
        float duration = offsetY / (OffsetSpeed * speedMulti);
        //Debug.Log("OffsetManager: Setting World Offset Y to " + offsetY + " over duration " + duration + " with speed factor " + speedMulti);
        Tween.PositionY(transform, nextPoint, duration, ease: Ease.InOutSine);
    }
    private void ResetWorldOffsetY()
    {
        Tween.StopAll(transform);
        if (WorldOffsetY > 0)
        {
            Tween.PositionY(transform, 0, ResetDuration, ease: Ease.InOutSine);
            WorldOffsetY = 0f;
        }
    }
}
