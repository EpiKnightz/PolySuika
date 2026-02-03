public class UIChangeTextOnSetEvent : UIChangeTextOnEvent<LevelSetSO>
{
    protected override void OnTriggerChange(LevelSetSO newSet)
    {
        if (newSet != null)
        {
            TextComponent.text = newSet.GetSetName();
        }
        PlayAnim();
    }
}
