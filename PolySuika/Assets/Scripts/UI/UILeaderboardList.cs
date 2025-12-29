using FirstGearGames.Utilities.Objects;
using Sortify;
using UnityEngine;

public class UILeaderboardList : MonoBehaviour
{
    [Header("References")]
    public RectTransform ListParent;

    [BetterHeader("Variables")]
    public GameObject EntryPrefab;

    [BetterHeader("Listen To")]
    public LeaderboardEventChannelSO ECOnLeaderboardUpdated;    

    private void Awake()
    {
        ECOnLeaderboardUpdated.Sub(OnLeaderboardUpdated);
    }

    private void OnDestroy()
    {
        ECOnLeaderboardUpdated.Unsub(OnLeaderboardUpdated);
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
