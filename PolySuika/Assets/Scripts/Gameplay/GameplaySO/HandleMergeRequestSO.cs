using Sortify;
using UnityEngine;

[CreateAssetMenu(fileName = "HandleMergeRequestSO", menuName = "GO/HandleMergeRequestSO")]
public class HandleMergeRequestSO : ScriptableObject
{
    [Header("Variables")]
    [SerializeField] protected int MaxTier = 7;
    [SerializeField] protected int OffsetProcessTier = 1;

    [BetterHeader("Broadcast On")]
    public IntEventChannelSO ECOnMergeEvent = null;
    public VectorEventChannelSO ECOnMergePosition = null;

    // Privates
    private Vector3 MergePosition;

    public virtual bool ValidateRequest(Mergable first, Mergable second)
    {
        return first.GetTier() < MaxTier;
    }

    public virtual int ProcessTierRequest(Mergable first, Mergable second)
    {
        return first.GetTier() + OffsetProcessTier;
    }

    public virtual Vector3 ProcessPositionRequest(Mergable first, Mergable second)
    {
        MergePosition = (first.transform.position + second.transform.position) / 2;
        return MergePosition;
    }

    public virtual void PreprocessRequest(Mergable first, Mergable second)
    {
        second.SetIsMerging(true);
        second.EnablePhysic(false);
        first.EnablePhysic(false);
        first.SetIsMerging(true);
    }

    public virtual void PostprocessRequest(Mergable first, Mergable second)
    {
        ECOnMergeEvent.Invoke(first.GetTier());
        ECOnMergePosition.Invoke(MergePosition);
    }
}
