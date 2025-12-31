using UnityEngine;
using FMODUnity;
using Sortify;

[RequireComponent(typeof(StudioEventEmitter))]
public class MusicManager : MonoBehaviour
{
    [Header("References")]
    public StudioEventEmitter EventEmitter;

    [BetterHeader("Listen To")]
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
