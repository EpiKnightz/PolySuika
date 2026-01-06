using PrimeTween;
using Sortify;
using UnityEngine;

public class CheckFull : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer warningMesh;

    [BetterHeader("Variables")]
    [SerializeField] private LayerMask MergableLayerMask;
    [SerializeField] private float maxCountdown = 5f;

    [BetterHeader("Broadcast On")]
    public VoidEventChannelSO ECOnLoseTrigger;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECOnRestartTriggered;

    private Material warningMaterial;
    private Tween warningTween;
    private bool bIsCountdown = false;
    private float currentCountdown = 0f;

    private void Start()
    {
        warningMaterial = warningMesh.material;
        warningMaterial.SetFloat("_Alpha", 0);
    }

    private void OnEnable()
    {
        ECOnRestartTriggered.Sub(ResetCountdown);
    }

    private void OnDisable()
    {
        ECOnRestartTriggered.Unsub(ResetCountdown);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Mergable>(out var mergable))
        {
            mergable.EOnDisable += CheckOnDisable;
            if (mergable.IsImpacted())
            {
                StartCountdown();
            }
            else
            {
                mergable.EOnImpact += CheckOnImpact;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Mergable>(out var mergable))
        {
            RayCheck();
            mergable.EOnImpact -= CheckOnImpact;
            mergable.EOnDisable -= CheckOnDisable;
        }
    }

    private void CheckOnImpact(Mergable mergable)
    {
        RayCheck();
        mergable.EOnImpact -= CheckOnImpact;
    }

    private void CheckOnDisable(Mergable mergable)
    {
        RayCheck();
        mergable.EOnDisable -= CheckOnDisable;
    }

    private bool RayCheck()
    {
        // Need to check if there are still mergables inside the trigger
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2f,
                                                    transform.rotation, MergableLayerMask);
        if (colliders.Length == 0)
        {
            // Stop countdown
            ResetCountdown();
            return false;
        }
        else
        {
            StartCountdown();
            return true;
        }
    }

    private void StartCountdown()
    {
        if (!bIsCountdown)
        {
            // Start countdown
            bIsCountdown = true;
            warningTween = Tween.Custom(1, 0, duration: 0.5f, ease: Ease.InOutSine, cycles: -1, cycleMode: CycleMode.Yoyo
                , onValueChange: newVal => { if (bIsCountdown) warningMaterial.SetFloat("_Alpha", newVal); });
        }
    }

    private void ResetCountdown()
    {
        bIsCountdown = false;
        currentCountdown = 0f;

        ResetLine();
    }

    private void ResetLine()
    {
        warningMaterial.SetFloat("_Alpha", 0);
        warningTween.Stop();
    }

    private void Update()
    {
        if (bIsCountdown)
        {
            currentCountdown += Time.deltaTime;
            if (currentCountdown > maxCountdown)
            {
                // Double check one final time
                if (RayCheck())
                {
                    ECOnLoseTrigger.Invoke();
                    ResetCountdown();
                }
            }
        }
    }
}