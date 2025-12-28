using UnityEngine;
using Utilities;

public class LeaderboardManager : MonoBehaviour
{
    private Leaderboard CurrentLeaderboard;

    // Events
    public event LeaderboardEvent EOnLeaderboardUpdated;
    private LeaderboardEvent DSaveLeaderboard2Disk;
    private GetLeaderboardEvent DGetLeaderboardFromDisk;

    void Start()
    {
        var SaveManager = FindAnyObjectByType<SaveManager>();
        if (SaveManager != null)
        {
            DSaveLeaderboard2Disk += SaveManager.SaveLeaderboard;
            DGetLeaderboardFromDisk += SaveManager.GetLeaderboard;
        }
        var ResetLeaderboardButton = FindAnyObjectByType<UIResetLeaderboardButton>();
        if (ResetLeaderboardButton != null)
        {
            ResetLeaderboardButton.EOnResetLeaderboardTriggered += ResetLeaderboard;
        }
        UpdateLeaderboardFromDisk();
    }


    public void UpdateLeaderboardFromDisk()
    {
        CurrentLeaderboard = DGetLeaderboardFromDisk?.Invoke();
        if (CurrentLeaderboard != null)
        {
            EOnLeaderboardUpdated?.Invoke(CurrentLeaderboard);
        }else
        {
            CurrentLeaderboard = new Leaderboard();
            DSaveLeaderboard2Disk?.Invoke(CurrentLeaderboard);
        }
    }

    public void OnNewEntryAdded(Entry entry)
    {
        CurrentLeaderboard.Add(entry);
        DSaveLeaderboard2Disk?.Invoke(CurrentLeaderboard);
        EOnLeaderboardUpdated?.Invoke(CurrentLeaderboard);
    }

    public void ResetLeaderboard()
    {
        CurrentLeaderboard.Clear();
        DSaveLeaderboard2Disk?.Invoke(CurrentLeaderboard);
        EOnLeaderboardUpdated?.Invoke(CurrentLeaderboard);
    }

    public Leaderboard GetCurrentLeaderboard() { return CurrentLeaderboard; }
}
