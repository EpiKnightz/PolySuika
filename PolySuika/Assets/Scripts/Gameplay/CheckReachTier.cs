using Sortify;
using UnityEngine;

public class CheckReachTier : MonoBehaviour
{
    [BetterHeader("Variables")]
    [SerializeField] private int TargetTier;
    [SerializeField] private float ExplodeInterval = 0.5f;

    [BetterHeader("Broadcast On")]
    public IntFloatEventChannelSO ECOnReachTargetTier = null;
    public VoidEventChannelSO ECOnGameOver = null;

    [BetterHeader("Listen To")]
    public IntEventChannelSO ECOnMergeEvent;

    private void OnEnable()
    {
        ECOnMergeEvent.Sub(CheckReachTargetTier);
    }

    private void OnDisable()
    {
        ECOnMergeEvent.Unsub(CheckReachTargetTier);
    }

    private void CheckReachTargetTier(int Tier)
    {
        if (Tier == TargetTier)
        {
            ECOnGameOver.Invoke();
            ECOnReachTargetTier.Invoke(TargetTier + 1, ExplodeInterval);
        }
    }
}
