using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EC_On", menuName = "SO/VoidEventChannelSO")]
public class VoidEventChannelSO : ScriptableObject
{
    private UnityAction EOnEvent;
    public void Invoke()
    {
        EOnEvent?.Invoke();
    }

    public void Sub(UnityAction action)
    {
        EOnEvent += action;
    }

    public void Unsub(UnityAction action)
    {
        EOnEvent -= action;
    }
}