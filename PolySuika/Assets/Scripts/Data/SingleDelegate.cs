using UnityEngine;
using UnityEngine.Events;

// Single Delegate restrict to one event added at a time
public class SingleDelegate
{
    private UnityAction EOnEvent;

    public void Invoke()
    {
        if (EOnEvent != null)
        {
            EOnEvent.Invoke();
        }
#if UNITY_EDITOR        
        else
        {           
            Debug.LogWarning("SingleDelegate: No delegate registered to invoke " + GetType());
        }
#endif        
    }

    public bool Reg(UnityAction action, bool forceReplace = false)
    {
        if (forceReplace)
        {
            EOnEvent = action;
            return true;
        }
        else if (EOnEvent == null)
        {
            EOnEvent = action;
            return true;
        }
        return false;
    }

    public void Unreg(UnityAction action)
    {
        EOnEvent -= action;
    }
}

public class SingleDelegate<T>
{
    private UnityAction<T> EOnEvent;

    public void Invoke(T arg)
    {
        if (EOnEvent != null)
        {
            EOnEvent.Invoke(arg);
        }
#if UNITY_EDITOR        
        else
        {           
            Debug.LogWarning("SingleDelegate: No delegate registered to invoke " + GetType());
        }
#endif
    }

    public bool Reg(UnityAction<T> action, bool forceReplace = false)
    {
        if (forceReplace)
        {
            EOnEvent = action;
            return true;
        }
        else if (EOnEvent == null)
        {
            EOnEvent = action;
            return true;
        }
        return false;
    }

    public void Unreg(UnityAction<T> action)
    {
        EOnEvent -= action;
    }
}

public class SingleDelegate<T0, T1>
{
    private UnityAction<T0, T1> EOnEvent;

    public void Invoke(T0 arg0, T1 arg1)
    {
        if (EOnEvent != null)
        {
            EOnEvent.Invoke(arg0, arg1);
        }
#if UNITY_EDITOR
        else
        {     
            Debug.LogWarning("SingleDelegate: No delegate registered to invoke " + GetType());
        }
#endif
    }

    public bool Reg(UnityAction<T0, T1> action, bool forceReplace = false)
    {
        if (forceReplace)
        {
            EOnEvent = action;
            return true;
        }
        else if (EOnEvent == null)
        {
            EOnEvent = action;
            return true;
        }
        return false;
    }

    public void Unreg(UnityAction<T0, T1> action)
    {
        EOnEvent -= action;
    }
}