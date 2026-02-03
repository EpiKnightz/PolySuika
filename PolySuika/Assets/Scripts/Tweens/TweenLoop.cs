using PrimeTween;
using UnityEngine;

public class TweenLoop<T> : MonoBehaviour where T : struct
{
    [Header("Variables")]
    [SerializeField] protected TweenSettings<T> Settings;

    // Privates
    protected Tween ObjectTween;

    protected virtual void Start()
    {
        Animate();
    }

    protected virtual void Animate()
    {
    }
}
