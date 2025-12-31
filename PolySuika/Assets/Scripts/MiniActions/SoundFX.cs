using FMODUnity;
using UnityEngine;

public class SoundFX : PlayFXOnEvent<Vector3>
{
    [Header("Variables")]
    public EventReference MergeSFX;

    protected override void OnMergeTriggered(Vector3 position)
    {
        RuntimeManager.PlayOneShot(MergeSFX, position);
    }
}
