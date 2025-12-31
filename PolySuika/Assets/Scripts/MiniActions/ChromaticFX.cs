using PrimeTween;
using Sortify;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChromaticFX : PlayFXOnEvent<int>
{
    [Header("References")]
    [SerializeField] private Volume PostprocessVolume;

    // Privates
    private ChromaticAberration chromaticAberration;

    private void Awake()
    {
        if (PostprocessVolume != null)
        {
            PostprocessVolume.profile.TryGet(out chromaticAberration);
        }
    }

    protected override void OnMergeTriggered(int value)
    {        
        if (chromaticAberration != null)
        {
            float intensity = 0.3f + 0.1f * value;
            Tween.Custom(0f, intensity, 0.125f, cycles: 2, cycleMode: CycleMode.Rewind, ease: Ease.InOutSine, onValueChange: newVal => {
                chromaticAberration.intensity.Override(newVal);
            });
        }
    }
}
