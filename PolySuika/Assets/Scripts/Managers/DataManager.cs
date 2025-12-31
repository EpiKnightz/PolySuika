using Reflex.Core;
using Sortify;
using UnityEngine;

public class DataManager : MonoBehaviour, IInstaller, IDataManager
{
    [Header("Variables")]
    [SerializeField] private LevelSet[] LevelSets;

    [BetterHeader("Broadcast On")]
    public IntEventChannelSO ECOnSetChange = null;
    public IntEventChannelSO ECOnCurrentLevelSetChangedOffset = null;

    [BetterHeader("Listen To")]
    public IntEventChannelSO ECOnChangeSetOffsetTriggered;

    // Private
    private int CurrentLevelSetIndex = 0;

    public void InstallBindings(ContainerBuilder builder)
    {
        builder.AddSingleton(this, typeof(IDataManager));
    }

    void OnEnable()
    {
        ECOnChangeSetOffsetTriggered.Sub(OffsetCurrentLevelSet);
    }

    void OnDisable()
    {
        ECOnChangeSetOffsetTriggered.Unsub(OffsetCurrentLevelSet);
    }

    public GameObject[] GetCurrentTierPrefabs()
    {
        return LevelSets[CurrentLevelSetIndex].TierPrefabs;
    }

    public float GetCurrentBaseScale()
    {
        return LevelSets[CurrentLevelSetIndex].BaseScale;
    }

    public float GetCurrentScaleIncrement()
    {
        return LevelSets[CurrentLevelSetIndex].ScaleIncrement;
    }

    public int GetCurrentSetIdx()
    {
        return CurrentLevelSetIndex;
    }

    public void OffsetCurrentLevelSet(int offset)
    {
        CurrentLevelSetIndex += offset;
        if (CurrentLevelSetIndex >= LevelSets.Length)
        {
            CurrentLevelSetIndex = 0;
        }
        if (CurrentLevelSetIndex < 0)
        {
            CurrentLevelSetIndex = LevelSets.Length - 1;
        }
        ECOnSetChange.Invoke(CurrentLevelSetIndex);
        ECOnCurrentLevelSetChangedOffset.Invoke(offset);
    }

    public GameObject GetCurrentShopBG()
    {
        return LevelSets[CurrentLevelSetIndex].ShopPrefab;
    }
}
