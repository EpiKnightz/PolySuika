using Sortify;
using UnityEngine;

public class RemoveColliderWhenLose : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider TargetCollider;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECOnLoseTrigger;    

    private void OnEnable()
    {
        ECOnLoseTrigger.Sub(OnLoseTrigger);
    }

    private void OnDisable()
    {
        ECOnLoseTrigger.Unsub(OnLoseTrigger);
    }

    void OnLoseTrigger()
    {
        TargetCollider.enabled = false;
    }
}
