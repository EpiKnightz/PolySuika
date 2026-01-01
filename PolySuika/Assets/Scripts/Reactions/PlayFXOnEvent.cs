using Sortify;
using UnityEngine;

public class PlayFXOnEvent<T> : MonoBehaviour
{
    [BetterHeader("Listen To")]
    public EventChannelSO<T> ECOnMergeTriggered;

    private void OnEnable()
    {
        ECOnMergeTriggered.Sub(OnMergeTriggered);
    }

    private void OnDisable()
    {
        ECOnMergeTriggered.Unsub(OnMergeTriggered);
    }

    protected virtual void OnMergeTriggered(T value) { }
}
