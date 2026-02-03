using UnityEngine;

public class PhysicMatModifier : PhysicModifier
{
    [Header("References")]
    [SerializeField] private PhysicsMaterial DefaultPhysics;

    [Header("Variables")]
    [SerializeField] private PhysicsMaterial PhysicsMaterial;

    // Private
    private PhysicsMaterial CachedMaterial;

    protected override void ApplyMod()
    {
        if (CachedMaterial == null)
        {
            CachedMaterial = new();
            CopyPhysicsValue(CachedMaterial, DefaultPhysics);
        }
        CopyPhysicsValue(DefaultPhysics, PhysicsMaterial);
    }

    protected override void DisableMod()
    {
        if (CachedMaterial != null)
        {
            CopyPhysicsValue(DefaultPhysics, CachedMaterial);
            CachedMaterial = null;
        }
    }

    private void CopyPhysicsValue(PhysicsMaterial a, PhysicsMaterial b)
    {
        a.staticFriction = b.staticFriction;
        a.dynamicFriction = b.dynamicFriction;
        a.bounciness = b.bounciness;
        a.frictionCombine = b.frictionCombine;
        a.bounceCombine = b.bounceCombine;
    }
}
