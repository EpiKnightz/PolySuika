using PrimeTween;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class CheckFull : MonoBehaviour
{
    public LayerMask MergableLayerMask;
    public float maxCountdown = 3f;
    public MeshRenderer warningMesh;

    public event VoidEvent EOnLoseTrigger;

    [Header("Listen To")]
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
        ECOnRestartTriggered.UnSub(ResetCountdown);
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

    void CheckOnImpact(Mergable mergable)
    {
        RayCheck();
        mergable.EOnImpact -= CheckOnImpact;
    }

    void CheckOnDisable(Mergable mergable)
    {
        RayCheck();
        mergable.EOnDisable -= CheckOnDisable;
    }

    bool RayCheck()
    {
        // Need to check if there are still mergables inside the trigger
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2f,
                                                    transform.rotation, MergableLayerMask);
        if (colliders.Length < 1)
        {
            // Stop countdown
            ResetCountdown();
            return false;
        } else
        {
            StartCountdown();
            return true;
        }        
    }

    void StartCountdown()
    {
        if (!bIsCountdown)
        {
            // Start countdown
            bIsCountdown = true;
            warningTween = Tween.Custom(1, 0, duration: 0.5f, ease: Ease.InOutSine, cycles: -1, cycleMode: CycleMode.Yoyo
                , onValueChange: newVal => { if (bIsCountdown) warningMaterial.SetFloat("_Alpha", newVal); });
        }
    }

    void ResetCountdown()
    {
        bIsCountdown = false;
        currentCountdown = 0f;

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
                    EOnLoseTrigger?.Invoke();
                    bIsCountdown = false;
                }
            }
        }
    }
}