using UnityEngine;

[CreateAssetMenu(fileName = "GameModeSO", menuName = "Scriptable Objects/GameModeSO")]
public class GameModeSO : ScriptableObject
{
    [SerializeField] private string ModeName;
    [SerializeField] private GameObject CheckPrefab;
    [SerializeField] private Sprite Icon;

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
}
