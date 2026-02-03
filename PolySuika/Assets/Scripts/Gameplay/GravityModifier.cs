using UnityEngine;

public class GravityModifier : PhysicModifier
{
    [Header("Variables")]
    //[SerializeField] private PhysicsMaterial PhysicsMaterial;
    [SerializeField] private Vector3 GravityMod;

    // Privates
    private Vector3 DefaultGravity = new(0, -9.81f, 0);

    protected override void ApplyMod()
    {
        Physics.gravity = GravityMod;
    }

    protected override void DisableMod()
    {
        Physics.gravity = DefaultGravity;
    }
}
