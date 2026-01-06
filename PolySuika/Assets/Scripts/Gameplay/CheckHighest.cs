using Sortify;
using UnityEngine;

public class CheckHighest : MonoBehaviour
{
    [BetterHeader("Variables")]
    [SerializeField] private LayerMask MergableLayerMask;
    [SerializeField] private string ImpactedTag;
    [SerializeField] private float CheckInterval = 10f;


    [BetterHeader("Broadcast On")]
    public FloatEventChannelSO ECOnTriggerOffsetWorldY;

    [BetterHeader("Listen To")]
    public IntEventChannelSO ECOnScoreTotalChange;
    public VoidEventChannelSO ECOnActionAnimFinished;
    public VoidEventChannelSO ECActionHidden;

    // Privates
    private float CurrentCheckCountdown = -1;

    private void OnEnable()
    {
        ECOnScoreTotalChange.Sub(CheckHighestY);
        ECOnActionAnimFinished.Sub(StartCooldown);
        ECActionHidden.Sub(StopCooldown);
        StopCooldown();
    }
    private void OnDisable()
    {
        ECOnScoreTotalChange.Unsub(CheckHighestY);
        ECOnActionAnimFinished.Unsub(StartCooldown);
        ECActionHidden.Unsub(StopCooldown);
        StopCooldown();
    }

    private void CheckHighestY(int newScore)
    {
        if (newScore > 0)
        {
            CheckHighestY();
        }
    }

    private void CheckHighestY()
    {
        StartCooldown();
        float offsetAmount = RayCheck();
        if (offsetAmount > 0)
        {
            ECOnTriggerOffsetWorldY.Invoke(offsetAmount);
        }
    }

    private float RayCheck()
    {
        float radiusY = transform.localScale.y / 2f;
        // Need to check if there are still mergables inside the trigger
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2f,
                                                    transform.rotation, MergableLayerMask);
        if (colliders.Length > 0)
        {
            // Find the highest point, calculate amount of down needed, then broadcast it
            float highestPoint = 0;
            foreach (var collider in colliders)
            {
                if (collider.gameObject.CompareTag(ImpactedTag))
                {
                    float colliderTop = collider.bounds.max.y;
                    if (colliderTop > highestPoint)
                    {
                        highestPoint = colliderTop;
                    }
                }
            }

            return highestPoint - transform.position.y + radiusY;
        }
        return 0;
    }

    private void Update()
    {
        if (CurrentCheckCountdown > 0)
        {
            CurrentCheckCountdown -= Time.deltaTime;
            if (CurrentCheckCountdown <= 0)
            {
                CheckHighestY();
            }
        }
    }

    private void StartCooldown()
    {
        CurrentCheckCountdown = CheckInterval;
    }

    private void StopCooldown()
    {
        CurrentCheckCountdown = -1;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ImpactedTag ??= "Impacted";
        MergableLayerMask = LayerMask.GetMask("Mergables");
    }
#endif
}
