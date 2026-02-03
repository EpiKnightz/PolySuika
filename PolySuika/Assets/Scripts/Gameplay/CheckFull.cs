using PrimeTween;
using Sortify;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CheckFull : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer WarningMesh;
    [SerializeField] private Collider ThisCollider;

    [BetterHeader("Variables")]
    [SerializeField] private LayerMask MergableLayerMask;
    [SerializeField] private float MaxCountdown = 5f;
    [SerializeField] private float DoubleCheckInterval = 1.01f;

    [BetterHeader("Broadcast On")]
    public VoidEventChannelSO ECOnLoseTrigger;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECOnRestartTriggered;

    private Material WarningMaterial;
    private Tween WarningTween;
    private bool IsCountdown = false;
    private float CurrentCountdown = 0f;
    private float CurrentCheckInterval = 0f;
    private Vector3 Radius;
    private Collider[] ColliderBuffer = new Collider[1];

    private void Awake()
    {
        WarningMaterial = WarningMesh.material;
        Radius = transform.localScale / 2f;
    }

    private void OnEnable()
    {
        ECOnRestartTriggered.Sub(RestartCheck);
        RestartCheck();
    }

    private void OnDisable()
    {
        ECOnRestartTriggered.Unsub(RestartCheck);
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
            //RayCheck();
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
        if (Physics.OverlapBoxNonAlloc(transform.position, Radius, ColliderBuffer,
                                                    transform.rotation, MergableLayerMask) == 0)
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
        if (!IsCountdown)
        {
            // Start countdown
            IsCountdown = true;
            WarningMesh.gameObject.SetActive(true);
            WarningTween = Tween.Custom(1, 0, duration: 0.5f, ease: Ease.InOutSine, cycles: -1, cycleMode: CycleMode.Yoyo
                , onValueChange: newVal =>
                {
                    WarningMaterial.color = new Color(WarningMaterial.color.r,
                                                        WarningMaterial.color.g,
                                                        WarningMaterial.color.b,
                                                        newVal);
                });
        }
    }

    private void RestartCheck()
    {
        ThisCollider.enabled = true;
        ResetCountdown();
    }

    private void ResetCountdown()
    {
        IsCountdown = false;
        CurrentCountdown = 0f;
        CurrentCheckInterval = 0f;
        ResetLine();
    }

    private void ResetLine()
    {
        WarningMaterial.color = new Color(WarningMaterial.color.r,
                                    WarningMaterial.color.g,
                                    WarningMaterial.color.b,
                                    0);
        WarningTween.Stop();
        WarningMesh.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (IsCountdown)
        {
            CurrentCountdown += Time.deltaTime;
            CurrentCheckInterval += Time.deltaTime;
            if (CurrentCountdown > MaxCountdown)
            {
                // Double check one final time
                if (RayCheck())
                {
                    ECOnLoseTrigger.Invoke();
                    ResetCountdown();
                    ThisCollider.enabled = false;
                }
            }
            else if (CurrentCheckInterval > DoubleCheckInterval)
            {
                RayCheck();
                CurrentCheckInterval = 0;
            }

        }
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