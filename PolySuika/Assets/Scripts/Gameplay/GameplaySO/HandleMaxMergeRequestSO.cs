using UnityEngine;

[CreateAssetMenu(fileName = "HandleMaxMergeRequestSO", menuName = "GO/HandleMaxMergeRequestSO")]
public class HandleMaxMergeRequestSO : HandleMergeRequestSO
{
    [Header("Variables")]
    [SerializeField] private int AlternateMaxTier = -100;

    public override bool ValidateRequest(Mergable first, Mergable second)
    {
        return true;
    }

    public override int ProcessTierRequest(Mergable first, Mergable second)
    {
        int result = first.GetTier() + OffsetProcessTier;
        return result > MaxTier ? AlternateMaxTier : result;
    }
}
