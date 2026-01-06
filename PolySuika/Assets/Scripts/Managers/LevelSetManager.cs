using Sortify;
using UnityEngine;
using Utilities;


public class LevelSetManager : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private LevelSet[] LevelSets;

    [BetterHeader("Broadcast On")]
    public IntEventChannelSO ECOnSetIndexOffset = null;
    public IntEventChannelSO ECOnSetIndexChange = null;
    public LevelSetEventChannelSO ECOnLevelSetChange = null;
    public VoidEventChannelSO ECOnRestartTriggered = null;

    [BetterHeader("Listen To")]
    public IntEventChannelSO ECOnChangeSetOffsetTriggered;

    // Private
    private int CurrentLevelSetIndex = 0;

    private void Start()
    {
        OffsetCurrentLevelSet(0);
    }

    private void OnEnable()
    {
        ECOnChangeSetOffsetTriggered.Sub(OffsetCurrentLevelSet);
    }

    private void OnDisable()
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
        ECOnSetIndexOffset.Invoke(offset);
        CurrentLevelSetIndex = ListUtilities.RepeatIndex(CurrentLevelSetIndex + offset, LevelSets);
        ECOnSetIndexChange.Invoke(CurrentLevelSetIndex);
        ECOnLevelSetChange.Invoke(LevelSets[CurrentLevelSetIndex]);
        ECOnRestartTriggered.Invoke();
    }

    public GameObject GetCurrentShopBG(int offset = 0)
    {
        if (offset != 0)
        {
            int newIndex = ListUtilities.RepeatIndex(CurrentLevelSetIndex + offset, LevelSets);
            return LevelSets[newIndex].ShopPrefab;
        }
        return LevelSets[CurrentLevelSetIndex].ShopPrefab;
    }
}
