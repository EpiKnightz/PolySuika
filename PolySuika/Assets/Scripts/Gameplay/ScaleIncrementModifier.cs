using Reflex.Attributes;

public class ScaleIncrementModifier : GameplayValueModifier<float>
{
    [Inject] private readonly IScaleIncrement ScaleIncrementManager;

    protected override float GetDefaultValue()
    {
        return ScaleIncrementManager.GetScaleIncrement();
    }

    protected override void SetNewValue(float newValue)
    {
        ScaleIncrementManager.SetScaleIncrement(newValue);
    }
}
