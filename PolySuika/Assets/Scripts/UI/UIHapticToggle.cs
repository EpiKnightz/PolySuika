using Solo.MOST_IN_ONE;

public class UIHapticToggle : UIToggle
{
    private void Awake()
    {
        if (!MOST_HapticFeedback.HapticsEnabled)
        {
            OnClick();
        }
    }

    public override void ToggleAction(bool enable)
    {
        MOST_HapticFeedback.HapticsEnabled = enable;
    }
}
