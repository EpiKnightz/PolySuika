using UnityEngine;
using Utilities;

public class DataManager : MonoBehaviour
{
    public LevelSet[] LevelSets;

    [Header("Broadcast On")]
    public IntEventChannelSO ECOnSetChange = null;
    public IntEventChannelSO ECOnCurrentLevelSetChangedOffset = null;

    // Private
    private int CurrentLevelSetIndex = 0;

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
        //EOnCurrentLevelSetChanged?.Invoke(CurrentLevelSetIndex);
        ECOnSetChange?.Invoke(CurrentLevelSetIndex);
        ECOnCurrentLevelSetChangedOffset?.Invoke(offset);
    }

    public GameObject GetCurrentShopBG()
    {
        return LevelSets[CurrentLevelSetIndex].ShopPrefab;
    }
}
