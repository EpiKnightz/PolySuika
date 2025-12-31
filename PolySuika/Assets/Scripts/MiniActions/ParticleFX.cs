using Lean.Pool;
using UnityEngine;

public class ParticleFX : PlayFXOnEvent<Vector3>
{
    [Header("Variables")]
    [SerializeField] private GameObject MergeVFXPrefab;
    protected override void OnMergeTriggered(Vector3 position)
    {
        if (MergeVFXPrefab != null)
        {
            var vfx = LeanPool.Spawn(MergeVFXPrefab,
                position,
                Quaternion.identity);
        }
    }
}
