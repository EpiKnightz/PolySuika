using Reflex.Attributes;
using Reflex.Core;
using Sortify;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour, ILeaderboardManager, IInstaller
{
    // Dependencies
    [Inject] private readonly ISaveManager SaveManager;
    [Inject] private readonly IPref PrefManager;

    // Events Channels
    [BetterHeader("Broadcast On")]
    public LeaderboardEventChannelSO ECOnLeaderboardUpdated = null;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECOnResetLeaderboardTriggered;
    public VoidEventChannelSO[] ECOnNeedUpdateLeaderboardEvent;
    public GameModeEventChannelSO ECOnCurrentModeChange;
    public LevelSetEventChannelSO ECOnLevelSetChange;

    // Private
    private Leaderboard CurrentLeaderboard;
    private bool IsUpdated = true;
    private int ModeID = -1;
    private int SetID = -1;

    public void InstallBindings(ContainerBuilder builder)
    {
        builder.AddSingleton(this, typeof(ILeaderboardManager));
    }

    private void OnEnable()
    {
        ECOnResetLeaderboardTriggered.Sub(ResetLeaderboard);
        for (int i = 0; i < ECOnNeedUpdateLeaderboardEvent.Length; i++)
        {
            ECOnNeedUpdateLeaderboardEvent[i].Sub(UpdateLeaderboardFromDisk);
        }
        ECOnCurrentModeChange.Sub(UpdateModeInfo);
        ECOnLevelSetChange.Sub(UpdateSetInfo);
    }

    private void OnDisable()
    {
        ECOnResetLeaderboardTriggered.Unsub(ResetLeaderboard);
        for (int i = 0; i < ECOnNeedUpdateLeaderboardEvent.Length; i++)
        {
            ECOnNeedUpdateLeaderboardEvent[i].Unsub(UpdateLeaderboardFromDisk);
        }
        ECOnCurrentModeChange.Unsub(UpdateModeInfo);
        ECOnLevelSetChange.Unsub(UpdateSetInfo);
    }

    private void Start()
    {
        BackwardCompatibleHandle();
        UpdateLeaderboardFromDisk();
    }

    private void UpdateModeInfo(GameModeSO modeSO)
    {
        ModeID = modeSO.GetID();
        IsUpdated = true;
    }

    private void UpdateSetInfo(LevelSetSO setSO)
    {
        SetID = setSO.GetID();
        IsUpdated = true;
    }

    private void UpdateLeaderboardFromDisk()
    {
        if (IsUpdated)
        {
            if (!HandleLoad())
            {
                CurrentLeaderboard = new Leaderboard();
                //HandleSave();
            }
            ECOnLeaderboardUpdated.Invoke(CurrentLeaderboard);
            IsUpdated = false;
        }
    }

    public bool CheckLeaderboardEligible(int score)
    {
        return CurrentLeaderboard.CompareLast(score);
    }

    public void AddLeaderboardEntry(Entry entry)
    {
        CurrentLeaderboard.Add(entry);
        SaveNewLeaderboard();
    }

    public void ResetLeaderboard()
    {
        CurrentLeaderboard.Clear();
        SaveNewLeaderboard(true);
    }

    private void SaveNewLeaderboard(bool isForce = false)
    {
        if (isForce
            || CurrentLeaderboard.entries.Count > 0)
        {
            HandleSave();
        }
        ECOnLeaderboardUpdated.Invoke(CurrentLeaderboard);
        IsUpdated = true;
    }

    private bool HandleLoad()
    {
        string suffix = HandleSuffix();
        return SaveManager.TryLoad(out CurrentLeaderboard, suffix);
    }

    private void BackwardCompatibleHandle()
    {
        string BackwardKey = "Backwardv15";
        if (!PrefManager.HasKey(BackwardKey))
        {
            if (SaveManager.TryLoad(out CurrentLeaderboard))
            {
                if (!HandleSave())
                {
                    return;
                }
            }
            PrefManager.SaveInt(BackwardKey, 1);
        }
    }

    private bool HandleSave()
    {
        string suffix = HandleSuffix();
        return SaveManager.Save(CurrentLeaderboard, suffix);
    }

    private string HandleSuffix()
    {
        return ModeID != -1
            && SetID != -1
            ? "_" + ModeID.ToString() + "_" + SetID.ToString()
            : "";
    }
}
