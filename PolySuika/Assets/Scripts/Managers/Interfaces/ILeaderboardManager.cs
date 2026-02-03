public interface ILeaderboardManager
{
    void AddLeaderboardEntry(Entry entry);
    void ResetLeaderboard();
    bool CheckLeaderboardEligible(int score);
}