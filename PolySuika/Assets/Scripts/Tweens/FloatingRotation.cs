using UnityEngine;
using PrimeTween;
public class FloatingRotation : MonoBehaviour
{
    public float floatEndpoint = 30;
    public float floatDuration = 2f;
    public float floatStartDelay = 0f;
    public float floatEndDelay = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Tween.Rotation(transform, new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, floatEndpoint), floatDuration, cycles: -1, cycleMode: CycleMode.Yoyo, ease: Ease.InOutSine,
            startDelay: floatStartDelay, endDelay: floatEndDelay);
    }
}
