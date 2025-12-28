using UnityEngine;
using Utilities;

public class UIRestartButton : MonoBehaviour
{
    [Header("Broadcast On")]
    public VoidEventChannelSO ECOnRestartTriggered = null;

   // public event VoidEvent EOnRestartTriggered;

    void Start()
    {
        //EOnRestartTriggered += TierManager.instance.ClearBoard;
    }

    public void OnClick()
    {
        //EOnRestartTriggered?.Invoke();
        ECOnRestartTriggered?.Invoke();
    }
}
