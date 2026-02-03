using Sortify;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    [BetterHeader("Broadcast On")]
    public VoidEventChannelSO ECOnClick = null;

    public virtual void OnClick()
    {
        ECOnClick.Invoke();
    }
}

public class UIButton<T> : MonoBehaviour
{
    [BetterHeader("Broadcast On")]
    public EventChannelSO<T> ECOnClick = null;

    public virtual void OnClick(T value)
    {
        ECOnClick.Invoke(value);
    }
}