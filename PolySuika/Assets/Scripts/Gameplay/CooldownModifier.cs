using Reflex.Attributes;

public class CooldownModifier : GameplayValueModifier<float>
{
    [Inject] private readonly ICooldown CooldownManager;

    protected override float GetDefaultValue()
    {
        return CooldownManager.GetCooldownTime();
    }

    protected override void SetNewValue(float newValue)
    {
        CooldownManager.SetCooldownTime(newValue);
    }
}
