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
    private Vector3 Radius;
    private Collider[] ColliderBuffer = new Collider[10];

    private void Awake()
    {
        Radius = transform.localScale / 2f;
    }

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
        //if (offsetAmount > 0)
        //{
        ECOnTriggerOffsetWorldY.Invoke(offsetAmount);
        //}
    }

    private float RayCheck()
    {
        // Need to check if there are still mergables inside the trigger
        int size = Physics.OverlapBoxNonAlloc(transform.position, Radius, ColliderBuffer,
                                                    transform.rotation, MergableLayerMask);
        if (size > 0)
        {
            // Find the highest point, calculate amount of down needed, then broadcast it
            float highestPoint = 0;
            for (int i = 0; i < size; i++)
            {
                if (ColliderBuffer[i].gameObject.CompareTag(ImpactedTag))
                {
                    float colliderTop = ColliderBuffer[i].bounds.max.y;
                    if (colliderTop > highestPoint)
                    {
                        highestPoint = colliderTop;
                    }
                }
            }
            return highestPoint + Radius.y - transform.position.y;
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
    }
#endif
}
