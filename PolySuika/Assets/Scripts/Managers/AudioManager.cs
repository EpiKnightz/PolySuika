using FMODUnity;
using Sortify;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]
public class AudioManager : MonoBehaviour
{
    [Header("References")]
    public StudioEventEmitter EventEmitter;

    [Header("Variables")]
    [SerializeField] private string[] BusNameList;
    //[BetterHeader("Broadcast On")]

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

    private void Start()
    {
        CheckMuteStatus();
    }

    async Awaitable CheckMuteStatus()
    {
        while (!RuntimeManager.HaveAllBanksLoaded)
        {
            await Awaitable.WaitForSecondsAsync(1.5f);
        }
        bool disable;
        string key;
        foreach (var busName in BusNameList)
        {
            key = busName[5..];
            disable = PlayerPrefs.HasKey(key) && PlayerPrefs.GetInt(key) == 1;
            RuntimeManager.GetBus(busName).setMute(disable);
        }
    }

    private void OnSetChange(int newIndex)
    {
        EventEmitter.SetParameter("Track", newIndex);
    }
}
