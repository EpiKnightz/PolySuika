using Sortify;
using UnityEngine;

public class UIResetCamOnClick : MonoBehaviour
{
    [BetterHeader("Broadcast On")]
    public FloatEventChannelSO ECResetLocalCam;

    [BetterHeader("Listen To")]
    public VectorEventChannelSO ECOnClick;

    public void Enable()
    {
        if (!enabled)
        {
            enabled = true;
            ECOnClick.Sub(ResetCamLocalPos);
        }
    }

    private void OnDisable()
    {
        if (!enabled)
        {
            ECOnClick.Unsub(ResetCamLocalPos);
        }
    }

    private void ResetCamLocalPos(Vector3 value)
    {
        ECResetLocalCam.Invoke(1);
        enabled = false;
        ECOnClick.Unsub(ResetCamLocalPos);
    }
}
