using UnityEngine;
using Utilities;

public class UIResetLeaderboardButton : MonoBehaviour
{
    public VoidEvent EOnResetLeaderboardTriggered;

    public void OnClick()
    {
        EOnResetLeaderboardTriggered?.Invoke();
    }
}
