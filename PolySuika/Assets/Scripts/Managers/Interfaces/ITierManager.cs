using UnityEngine;

public interface ITierManager
{
    int GetMaxTier();
    void OnClick(Vector3 offsetPosition);
    void PoppingUp(GameObject Object, int Tier);
    void ResetTier();
    void ReturnMergable(Mergable script, bool removeFromList = true);
    void ReturnTierRefs();
    GameObject SpawnAdvance(int Tier, Vector3 offsetPosition, bool popup = true, bool usePrefabZ = true);
}