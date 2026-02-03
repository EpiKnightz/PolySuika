using Reflex.Attributes;

public class MergeHandlerModifier : GameplayValueModifier<HandleMergeRequestSO>
{
    [Inject] private readonly IMergeHandler MergeHandlerManager;

    protected override HandleMergeRequestSO GetDefaultValue()
    {
        return MergeHandlerManager.GetMergeHandler();
    }

    protected override void SetNewValue(HandleMergeRequestSO newValue)
    {
        MergeHandlerManager.SetMergeHandler(newValue);
    }
}
