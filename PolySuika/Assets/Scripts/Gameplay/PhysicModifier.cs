using UnityEngine;

public class PhysicModifier : MonoBehaviour
{
    private void OnEnable()
    {
        ApplyMod();
    }

    private void OnDisable()
    {
        DisableMod();
    }

    protected virtual void ApplyMod()
    {
    }

    protected virtual void DisableMod()
    {
    }
}
