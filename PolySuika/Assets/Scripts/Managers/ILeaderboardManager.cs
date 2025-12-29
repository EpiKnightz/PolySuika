public interface ILeaderboardManager
{
    void AddLeaderboardEntry(Entry entry);
    Leaderboard GetCurrentLeaderboard();
    void ResetLeaderboard();
    void UpdateLeaderboardFromDisk();
}