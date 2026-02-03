using PrimeTween;
using Sortify;
using UnityEngine;

public class OffsetWorldObject : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] protected float OffsetDuration = 2f;
    [SerializeField] protected float ResetDuration = 0.5f;

    [BetterHeader("Broadcast On")]
    public FloatEventChannelSO ECOnAnimCompleted;

    [BetterHeader("Listen To")]
    public FloatEventChannelSO ECOnOffsetY;
    public VoidEventChannelSO ECOnResetOffset;

    // Privates

    private void OnEnable()
    {
        ECOnOffsetY.Sub(SetOffsetY);
        if (ECOnResetOffset != null)
        {
            ECOnResetOffset.Sub(ResetOffsetY);
        }
    }

    private void OnDisable()
    {
        ECOnOffsetY.Unsub(SetOffsetY);
        if (ECOnResetOffset != null)
        {
            ECOnResetOffset.Unsub(ResetOffsetY);
        }
    }

    protected virtual float GetOffsetY()
    {
        return transform.position.y;
    }

    protected virtual void SetOffsetY(float offsetY)
    {
        Tween.StopAll(transform);
        if (offsetY > 0
            && offsetY != GetOffsetY())
        {
            TweenOffset(offsetY, OffsetDuration).OnUpdate(target: this, (target, tween) => target.InvokeOffsetEvent());
        }
    }

    protected virtual void ResetOffsetY()
    {
        Tween.StopAll(transform);
        if (GetOffsetY() > 0)
        {
            TweenOffset(0, ResetDuration);
            InvokeOffsetEvent();
        }
    }

    protected virtual Tween TweenOffset(float location, float duration)
    {
        return Tween.PositionY(transform, location, duration, ease: Ease.InOutSine);
    }


    protected void InvokeOffsetEvent()
    {
        if (ECOnAnimCompleted != null)
        {
            ECOnAnimCompleted.Invoke(GetOffsetY());
        }
    }
}
