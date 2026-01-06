using Reflex.Attributes;
using Reflex.Core;
using Sortify;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour, ILeaderboardManager, IInstaller
{
    // Dependencies
    [Inject] private readonly ISaveManager SaveManager;

    // Events Channels
    [BetterHeader("Broadcast On")]
    public LeaderboardEventChannelSO ECOnLeaderboardUpdated = null;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECOnResetLeaderboardTriggered;
    public VoidEventChannelSO ECOnLeaderboardButtonTriggered;

    // Private
    private Leaderboard CurrentLeaderboard;
    private bool IsUpdated = true;

    public void InstallBindings(ContainerBuilder builder)
    {
        builder.AddSingleton(this, typeof(ILeaderboardManager));
    }

    private void OnEnable()
    {
        ECOnResetLeaderboardTriggered.Sub(ResetLeaderboard);
        ECOnLeaderboardButtonTriggered.Sub(UpdateLeaderboardFromDisk);
    }

    private void OnDisable()
    {
        ECOnResetLeaderboardTriggered.Unsub(ResetLeaderboard);
        ECOnLeaderboardButtonTriggered.Unsub(UpdateLeaderboardFromDisk);
    }

    private void Start()
    {
        UpdateLeaderboardFromDisk();
    }

    public void UpdateLeaderboardFromDisk()
    {
        if (IsUpdated)
        {
            CurrentLeaderboard = SaveManager.Load<Leaderboard>();
            if (CurrentLeaderboard != null)
            {
                ECOnLeaderboardUpdated.Invoke(CurrentLeaderboard);
            }
            else
            {
                CurrentLeaderboard = new Leaderboard();
                SaveManager.Save(CurrentLeaderboard);
            }
            IsUpdated = false;
        }
    }

    public bool CheckLeaderboardEligable(int score)
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
        SaveNewLeaderboard();
    }

    private void SaveNewLeaderboard()
    {
        SaveManager.Save(CurrentLeaderboard);
        ECOnLeaderboardUpdated.Invoke(CurrentLeaderboard);
        IsUpdated = true;
    }

    public Leaderboard GetCurrentLeaderboard() { return CurrentLeaderboard; }
}
