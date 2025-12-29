using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(StudioEventEmitter))]
public class MusicManager : MonoBehaviour
{
    [Header("Static Mapping")]
    public StudioEventEmitter EventEmitter;

    [Header("Listen To")]
    public IntEventChannelSO ECOnSetChange;

    private void OnEnable()
    {
        ECOnSetChange.Sub(OnSetChange);
    }

    private void OnDisable()
    {
        ECOnSetChange.Unsub(OnSetChange);
    }

    void OnSetChange(int newIndex)
    {
        EventEmitter.SetParameter("Track", newIndex);
    }
}
