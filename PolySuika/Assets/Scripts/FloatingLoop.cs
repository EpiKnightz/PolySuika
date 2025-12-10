using PrimeTween;
using UnityEngine;

public class FloatingLoop : MonoBehaviour
{
    public float floatEndpoint = 200;
    public float floatDuration = 2f;
    public float floatStartDelay = 0f;
    public float floatEndDelay = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Tween.PositionX(transform, floatEndpoint, floatDuration, cycles: -1, cycleMode: CycleMode.Restart, ease: Ease.Linear, 
            startDelay: floatStartDelay, endDelay: floatEndDelay);
    }
}
