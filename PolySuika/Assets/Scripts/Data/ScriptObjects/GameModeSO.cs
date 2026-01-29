using UnityEngine;

[CreateAssetMenu(fileName = "GameModeSO", menuName = "Data/GameModeSO")]
public class GameModeSO : ScriptableObject
{
    [SerializeField] private string ModeName;
    [SerializeField] private GameObject CheckPrefab;
    [SerializeField] private Sprite Icon;
    [Header("Unique & Once")]
    [SerializeField] private int ID;

    public string GetModeName()
    {
        return ModeName;
    }

    public GameObject GetCheckPrefab()
    {
        return CheckPrefab;
    }

    public Sprite GetIcon()
    {
        return Icon;
    }

    public int GetID()
    {
        return ID;
    }
}
