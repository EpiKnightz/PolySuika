using PrimeTween;

public class OffsetLocalObject : OffsetWorldObject
{
    protected override float GetOffsetY()
    {
        return transform.localPosition.y;
    }

    protected override Tween TweenOffset(float location, float duration)
    {
        return Tween.LocalPositionY(transform, location, duration, ease: Ease.InOutSine);
    }
}
