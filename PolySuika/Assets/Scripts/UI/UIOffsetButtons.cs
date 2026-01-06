using PrimeTween;
using Sortify;
using UnityEngine;

public class UIOffsetButtons : MonoBehaviour
{
    public float ButtonClickCooldown = 1;

    [BetterHeader("Broadcast On")]
    public IntEventChannelSO ECOnChangeOffsetTriggered = null;

    // Private
    private bool ClickEnable = true;

    public void OnClick(int Offset)
    {
        if (ClickEnable)
        {
            ECOnChangeOffsetTriggered.Invoke(Offset);
            ClickEnable = false;
            Tween.Delay(ButtonClickCooldown, EnableClick);
        }
    }

    public void EnableClick()
    {
        ClickEnable = true;
    }
}
