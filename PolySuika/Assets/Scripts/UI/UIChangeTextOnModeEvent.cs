public class UIChangeTextOnModeEvent : UIChangeTextOnEvent<GameModeSO>
{
    protected override void OnTriggerChange(GameModeSO newMode)
    {
        if (newMode != null)
        {
            TextComponent.text = newMode.GetModeName();
        }
        PlayAnim();
    }
}
