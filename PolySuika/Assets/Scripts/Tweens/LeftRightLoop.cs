using PrimeTween;
using UnityEngine;

public class LeftRightLoop : MonoBehaviour
{
    [Header("Variables")]
    public float Endpoint = 200;
    public float Duration = 2f;
    public float EndDelay = 0f;

    // Privates
    protected Tween ObjectTween;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        Animate();
    }

    protected virtual void Animate()
    {
        ObjectTween = Tween.LocalPositionX(transform, Endpoint, Duration, cycles: -1, cycleMode: CycleMode.Yoyo, ease: Ease.InOutSine,
                                endDelay: EndDelay);
    }
}
