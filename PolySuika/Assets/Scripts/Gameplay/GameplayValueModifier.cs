using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

public class GameplayValueModifier<T> : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private T ValueReplacement;

    // Private
    private T DefaultValue;

    private void Awake()
    {
        AttributeInjector.Inject(this, gameObject.scene.GetSceneContainer());
    }

    private void OnEnable()
    {
        DefaultValue = GetDefaultValue();
        SetNewValue(ValueReplacement);
    }

    private void OnDisable()
    {
        SetNewValue(DefaultValue);
        DefaultValue = default;
    }

    protected virtual T GetDefaultValue()
    {
        return default;
    }

    protected virtual void SetNewValue(T newValue)
    {

    }
}
