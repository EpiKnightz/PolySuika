using PrimeTween;
using UnityEngine;

public class RotateTweenLoop : TweenLoop<Vector3>
{
    protected override void Animate()
    {
        Tween.LocalEulerAngles(transform, Settings);
    }
}
