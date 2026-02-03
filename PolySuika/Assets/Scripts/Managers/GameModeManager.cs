using Lean.Pool;
using Sortify;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GameModeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform OffsetableTransform;

    [Header("Variables")]
    [SerializeField] private GameModeSO[] GameModeList;

    [BetterHeader("Broadcast On")]
    public GameModeEventChannelSO ECOnCurrentModeChange = null;
    public IntEventChannelSO ECOnGameModeIndexOffset = null;
    public VoidEventChannelSO ECOnRestartTriggered = null;

    [BetterHeader("Listen To")]
    public IntEventChannelSO ECOnChangeModeOffsetTriggered;
    public VoidEventChannelSO ECOnActionButtonTriggered;

    // Privates
    private int CurrentGameModeIndex = 0;
    private GameObject CurrentCheck;

    private void OnEnable()
    {
        ECOnChangeModeOffsetTriggered.Sub(OffsetCurrentGameMode);
        ECOnActionButtonTriggered.Sub(SpawnCheck);
    }

    private void OnDisable()
    {
        ECOnChangeModeOffsetTriggered.Unsub(OffsetCurrentGameMode);
        ECOnActionButtonTriggered.Unsub(SpawnCheck);
    }

    private void Start()
    {
        OffsetCurrentGameMode(0);
    }

    private void SpawnCheck()
    {
        GameObject checkPrefab = GameModeList[CurrentGameModeIndex].GetCheckPrefab();
        if (CurrentCheck != null)
        {
            if (CurrentCheck.name == checkPrefab.name)
            {
                return;
            }
            else
            {
                LeanPool.Despawn(CurrentCheck);
            }
        }
        CurrentCheck = LeanPool.Spawn(checkPrefab, checkPrefab.transform.position + OffsetableTransform.position, Quaternion.identity, OffsetableTransform); // So it would spawn at local
    }

    public void OffsetCurrentGameMode(int offset)
    {
        ECOnGameModeIndexOffset.Invoke(offset);
        CurrentGameModeIndex = ListUtilities.RepeatIndex(CurrentGameModeIndex + offset, GameModeList);
        ECOnCurrentModeChange.Invoke(GameModeList[CurrentGameModeIndex]);
        ECOnRestartTriggered.Invoke();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HashSet<int> CheckUnique = new();
        foreach (var gameMode in GameModeList)
        {
            if (!CheckUnique.Contains(gameMode.GetID()))
            {
                CheckUnique.Add(gameMode.GetID());
            }
            else
            {
                Debug.LogError("Duplicate ID in " + gameMode.name);
            }
        }
    }
#endif
}
