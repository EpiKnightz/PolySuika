using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Sets/LevelSet", order = 1)]
public class LevelSet: ScriptableObject
{
    public GameObject[] TierPrefabs;
    public float BaseScale = 1f;
    public float ScaleIncrement = 0.35f;

    public GameObject ShopPrefab;
}
