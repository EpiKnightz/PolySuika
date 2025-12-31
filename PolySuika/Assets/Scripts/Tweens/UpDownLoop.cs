using PrimeTween;
using UnityEngine;

public class UpDownLoop : MonoBehaviour
{
    [Header("Variables")]
    public float UpEndpoint = 200;
    public float upDownDuration = 2f;
    public float inactiveDelay = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Tween.LocalPositionY(transform, UpEndpoint, upDownDuration, cycles: -1, cycleMode: CycleMode.Yoyo, ease: Ease.InOutSine,
            endDelay: inactiveDelay);
    }
}
