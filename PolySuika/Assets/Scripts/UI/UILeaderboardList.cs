using FirstGearGames.Utilities.Objects;
using UnityEngine;

public class UILeaderboardList : MonoBehaviour
{
    // Static Linking
    public RectTransform ListParent;

    // Variables
    public GameObject EntryPrefab;

    private void Awake()
    {
        var leaderboardManager = FindAnyObjectByType<LeaderboardManager>();
        if (leaderboardManager != null)
        {
            leaderboardManager.EOnLeaderboardUpdated += OnLeaderboardUpdated;
        }
    }

    public void OnLeaderboardUpdated(Leaderboard leaderboard)
    {
        if (ListParent != null)
        {
            if (ListParent.childCount > 0)
            {
                ListParent.DestroyChildren();
            }
            for (int i = 0; i < leaderboard.entries.Count; i++)
            {
                if (EntryPrefab != null)
                {
                    var uIObject = Instantiate(EntryPrefab, ListParent);
                    var uIEntry = uIObject.GetComponent<UIEntry>();
                    if (uIEntry != null)
                    {
                        uIEntry.UpdateEntry(leaderboard.entries[i], i);
                    }
                }
            }
        }
    }
}
