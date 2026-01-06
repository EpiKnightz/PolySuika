using Lean.Pool;
using Sortify;
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

    private void SpawnCheck()
    {
        if (CurrentCheck != null)
        {
            LeanPool.Despawn(CurrentCheck);
        }
        GameObject checkPrefab = GameModeList[CurrentGameModeIndex].GetCheckPrefab();
        CurrentCheck = LeanPool.Spawn(checkPrefab, checkPrefab.transform.position + OffsetableTransform.position, Quaternion.identity, OffsetableTransform); // So it would spawn at local
    }

    public void OffsetCurrentGameMode(int offset)
    {
        ECOnGameModeIndexOffset.Invoke(offset);
        CurrentGameModeIndex = ListUtilities.RepeatIndex(CurrentGameModeIndex + offset, GameModeList);
        ECOnCurrentModeChange.Invoke(GameModeList[CurrentGameModeIndex]);
        ECOnRestartTriggered.Invoke();
    }
}
