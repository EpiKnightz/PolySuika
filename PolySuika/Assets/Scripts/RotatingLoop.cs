using PrimeTween;
using UnityEngine;

public class RotatingLoop : MonoBehaviour
{
    public float rotateEnd = 360;
    public float rotateDuration = 1f;
    public float inactiveDelay = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Tween.LocalEulerAngles(transform, Vector3.zero, new Vector3(rotateEnd, 0, 0), rotateDuration, cycles: -1, cycleMode: CycleMode.Restart, ease: Ease.Linear,
            endDelay: inactiveDelay);
    }
}
