using UnityEngine;

public interface IDataManager
{
    float GetCurrentBaseScale();
    float GetCurrentScaleIncrement();
    int GetCurrentSetIdx();
    GameObject GetCurrentShopBG();
    GameObject[] GetCurrentTierPrefabs();
    void OffsetCurrentLevelSet(int offset);
}