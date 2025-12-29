using UnityEngine;
using PrimeTween;
using Sortify;

public class UIChangeSetButtons : MonoBehaviour
{
    public float ButtonClickCooldown = 1;
    
    [BetterHeader("Broadcast On")]
    public IntEventChannelSO ECOnChangeSetOffsetTriggered = null;

    // Private
    private bool ClickEnable = true;

    public  void OnClick(int Offset)
    {
        if (ClickEnable)
        {
            ECOnChangeSetOffsetTriggered.Invoke(Offset);
            ClickEnable = false;
            Tween.Delay(ButtonClickCooldown, EnableClick);
        }
    }

    public void EnableClick()
    {
        ClickEnable = true; 
    }
}
