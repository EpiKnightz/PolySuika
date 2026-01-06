using FMODUnity;
using Sortify;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]
public class MusicManager : MonoBehaviour
{
    [Header("References")]
    public StudioEventEmitter EventEmitter;

    [BetterHeader("Listen To")]
    public IntEventChannelSO ECOnSetIndexChange;

    private void OnEnable()
    {
        ECOnSetIndexChange.Sub(OnSetChange);
    }

    private void OnDisable()
    {
        ECOnSetIndexChange.Unsub(OnSetChange);
    }

    private void OnSetChange(int newIndex)
    {
        EventEmitter.SetParameter("Track", newIndex);
    }
}
