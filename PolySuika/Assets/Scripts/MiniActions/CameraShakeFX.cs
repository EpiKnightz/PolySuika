using FirstGearGames.SmoothCameraShaker;
using Sortify;
using UnityEngine;
using Utilities;

public class CameraShakeFX : PlayFXOnEvent<int>
{
    [Header("Variables")]
    public ShakeData[] MergeCamFX;

    protected override void OnMergeTriggered(int value)
    {
        // Might increase the shake intensity based on Tier?
        int shakeIntensityIdx = 0;
        if (MergeCamFX.Length > 0)
        {
            if (MergeCamFX.Length == 3)
            {
                shakeIntensityIdx = value < GConst.TIER_RANK_1 ? 0 : value < GConst.TIER_RANK_2 ? 1 : 2;
            }
            CameraShakerHandler.Shake(MergeCamFX[shakeIntensityIdx]);
        }
    }
}
