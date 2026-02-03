using Sortify;
using UnityEngine;
using Utilities;

[RequireComponent(typeof(Collider))]
public class CheckOnEnter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider ThisCollider;

    [BetterHeader("Variables")]
    [SerializeField] private LayerMask MergableLayerMask;

    [BetterHeader("Broadcast On")]
    public IntEventChannelSO ECOnGoalCountUpdated = null;
    public VoidEventChannelSO ECOnGoalScore = null;
    public VectorEventChannelSO ECOnGoalScorePosition = null;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECOnBallReset;

    // Privates
    private int GoalCount = 0;

    private void OnEnable()
    {
        ECOnBallReset.Sub(OnBallReset);
    }

    private void OnDisable()
    {
        ECOnBallReset.Unsub(OnBallReset);
    }

    void OnBallReset()
    {
        ThisCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (MergableLayerMask.HaveLayer(other.gameObject))
        {
            ECOnGoalScore.Invoke();
            GoalCount++;
            ECOnGoalCountUpdated.Invoke(GoalCount);
            ECOnGoalScorePosition.Invoke(other.transform.position);

            ThisCollider.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
    }

    void ResetGoal()
    {
        GoalCount = 0;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (ThisCollider == null)
        {
            ThisCollider = GetComponent<Collider>();
        }
    }
#endif
}