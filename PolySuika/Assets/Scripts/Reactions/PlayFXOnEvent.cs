using UnityEngine;

public class PlayFXOnEvent<T> : MonoBehaviour
{
    [Header("Listen To")]
    public EventChannelSO<T>[] ECOnEventTriggered;

    private void OnEnable()
    {
        for (int i = 0; i < ECOnEventTriggered.Length; i++)
        {
            ECOnEventTriggered[i].Sub(OnMergeTriggered);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < ECOnEventTriggered.Length; i++)
        {
            ECOnEventTriggered[i].Unsub(OnMergeTriggered);
        }
    }

    protected virtual void OnMergeTriggered(T value) { }
}
