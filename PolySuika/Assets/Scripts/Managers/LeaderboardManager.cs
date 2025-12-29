using Reflex.Attributes;
using UnityEngine;
using Sortify;
using Reflex.Core;

public class LeaderboardManager : MonoBehaviour, ILeaderboardManager, IInstaller
{    
    // Dependencies
    [Inject] private readonly ISaveManager SaveManager;

    // Events Channels
    [BetterHeader("Broadcast On")]
    public LeaderboardEventChannelSO ECOnLeaderboardUpdated = null;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECOnResetLeaderboardTriggered;

    // Private
    private Leaderboard CurrentLeaderboard;

    private void Start()
    {
        // Temporary for now
        UpdateLeaderboardFromDisk();
    }

    public void InstallBindings(ContainerBuilder builder)
    {
        builder.AddSingleton(this, typeof(ILeaderboardManager));
    }

    private void OnEnable()
    {
        ECOnResetLeaderboardTriggered.Sub(ResetLeaderboard);
    }

    private void OnDisable()
    {
        ECOnResetLeaderboardTriggered.Unsub(ResetLeaderboard);
    }

    public void UpdateLeaderboardFromDisk()
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
    }

    public void AddLeaderboardEntry(Entry entry)
    {
        CurrentLeaderboard.Add(entry);
        SaveManager.Save(CurrentLeaderboard);
        ECOnLeaderboardUpdated.Invoke(CurrentLeaderboard);
    }

    public void ResetLeaderboard()
    {
        CurrentLeaderboard.Clear();
        SaveManager.Save(CurrentLeaderboard);
        ECOnLeaderboardUpdated.Invoke(CurrentLeaderboard);
    }

    public Leaderboard GetCurrentLeaderboard() { return CurrentLeaderboard; }
}
