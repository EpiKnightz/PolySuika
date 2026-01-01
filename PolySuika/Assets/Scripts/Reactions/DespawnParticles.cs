using Lean.Pool;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DespawnParticles : MonoBehaviour
{
    public void OnParticleSystemStopped()
    {
        LeanPool.Despawn(gameObject);
    }
}
