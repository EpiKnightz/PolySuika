using UnityEngine;
using UnityEngine.Events;

public class SingleDelegate
{
    private UnityAction EOnEvent;

    public void Invoke()
    {
        EOnEvent.Invoke();
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
        EOnEvent.Invoke(arg);
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
        else
        {
            Debug.LogWarning("SingleDelegate: No delegate registered to invoke " + GetType());
        }
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