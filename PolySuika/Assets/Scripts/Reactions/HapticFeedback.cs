using Solo.MOST_IN_ONE;
using Utilities;

public class HapticFeedback : PlayFXOnEvent<int>
{
    protected override void OnMergeTriggered(int value)
    {
        if (value < GConst.TIER_RANK_1)
        {
            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.LightImpact);
        }
        else if (value < GConst.TIER_RANK_2)
        {
            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.MediumImpact);
        }
        else if (value < GConst.TIER_RANK_3)
        {
            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.HeavyImpact);
        }
        else
        {
            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.Failure);
        }
    }
}
