using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Sets/LevelSet", order = 1)]
public class LevelSetSO : ScriptableObject
{
    public string SetName;
    public GameObject[] TierPrefabs;
    public float BaseScale = 1f;
    public float ScaleIncrement = 0.35f;
    public float RefScale = 0.5f;

    public GameObject ShopPrefab;
    [Header("Unique & Once")]
    [SerializeField] private int ID;

    public string GetSetName()
    {
        return SetName;
    }

    public int GetID()
    {
        return ID;
    }
}
