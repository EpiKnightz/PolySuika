using PrimeTween;
using Sortify;
using UnityEngine;

public class GoingDownOnEvent : MonoBehaviour
{
    [Header("Variables")]
    public float DownSpeed = 0.01f;

    [BetterHeader("Listen To")]
    public FloatEventChannelSO ECOnNeedWholeSceneDown;
    public VoidEventChannelSO ECOnRestartTriggered;

    private void OnEnable()
    {
        ECOnNeedWholeSceneDown.Sub(StartGoingDown);
        ECOnRestartTriggered.Sub(Reset);
    }

    private void OnDisable()
    {
        ECOnNeedWholeSceneDown.Unsub(StartGoingDown);
        ECOnRestartTriggered.Unsub(Reset);
    }

    private void StartGoingDown(float downAmount)
    {
        Tween.StopAll(transform);
        float nextPoint = transform.position.y - downAmount;
        float duration = downAmount / (DownSpeed * Mathf.CeilToInt(downAmount));
        Tween.LocalPositionY(transform, nextPoint, duration, ease: Ease.InOutSine);
    }

    private void Reset()
    {
        Tween.StopAll(transform);
        transform.position = Vector3.zero;
    }
}
