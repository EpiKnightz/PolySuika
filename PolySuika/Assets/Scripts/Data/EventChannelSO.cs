using UnityEngine;
using UnityEngine.Events;

//[CreateAssetMenu(fileName = "EventChannelSO", menuName = "SO/EventChannelSO")]
public class EventChannelSO<T> : ScriptableObject
{
    private UnityAction<T> EOnEvent;
    
    public void Invoke(T arg)
    {
        EOnEvent?.Invoke(arg);
    }

    public void Sub(UnityAction<T> action)
    {
        EOnEvent += action;
    }

    public void Unsub(UnityAction<T> action)
    {
        EOnEvent -= action;
    }
}

public class EventChannelSO<T0, T1> : ScriptableObject
{
    private UnityAction<T0, T1> EOnEvent;

    public void Invoke(T0 arg0, T1 arg1)
    {
        EOnEvent?.Invoke(arg0, arg1);
    }

    public void Sub(UnityAction<T0, T1> action)
    {
        EOnEvent += action;
    }

    public void UnSub(UnityAction<T0, T1> action)
    {
        EOnEvent -= action;
    }
}