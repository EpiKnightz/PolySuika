using Sortify;
using UnityEngine;
using Utilities;

public class UIButton : MonoBehaviour
{
    [BetterHeader("Broadcast On")]
    public VoidEventChannelSO ECOnClick = null;

    public virtual void OnClick()
    {
        ECOnClick.Invoke();
    }
}
