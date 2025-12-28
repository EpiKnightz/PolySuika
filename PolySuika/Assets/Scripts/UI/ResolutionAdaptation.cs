using UnityEngine;
using Sortify;

[RequireComponent(typeof(RectTransform))]
public class ResolutionAdaptation : MonoBehaviour
{
    public RectTransform TargetTransform;
    public float AdaptPercentage = 0.5f;

    [BetterHeader("Listen To")]
    public IntEventChannelSO ECResolutionChange;

    private void Awake()
    {
        if (TargetTransform == null)
        {
            TargetTransform = GetComponent<RectTransform>();
        }
        ECResolutionChange.Sub(OnResolutionChange);
    }

    private void OnDestroy()
    {
        ECResolutionChange.Unsub(OnResolutionChange);
    }

    void OnResolutionChange(int resolution)
    {
        RectTransformExtensions.SetHeight(TargetTransform, resolution * AdaptPercentage);
    }
}
