using UnityEngine;
using Utilities;

[RequireComponent(typeof(Collider))]
public class SlowTimeOnTrigger : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Collider ThisCollider;

    [Header("Variables")]
    [SerializeField] private LayerMask MergableLayerMask;
    [SerializeField] private float SlowTime = 0.1f;
    [SerializeField] private float FixedUpdateTime = 0.0125f;

    [Header("Broadcast On")]
    public VectorEventChannelSO ECOnSlowTimePosition = null;

    [Header("Listen To")]
    public VoidEventChannelSO[] ECEnableEventList;
    public VoidEventChannelSO[] ECDisableEventList;

    private void OnEnable()
    {
        for (int i = 0; i < ECEnableEventList.Length; ++i)
        {
            ECEnableEventList[i].Sub(EnableTrigger);
        }

        for (int i = 0; i < ECDisableEventList.Length; i++)
        {
            ECDisableEventList[i].Sub(DisableTrigger);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < ECEnableEventList.Length; ++i)
        {
            ECEnableEventList[i].Unsub(EnableTrigger);
        }

        for (int i = 0; i < ECDisableEventList.Length; i++)
        {
            ECDisableEventList[i].Unsub(DisableTrigger);
        }
    }

    void EnableTrigger()
    {
        ThisCollider.enabled = true;
    }

    void DisableTrigger()
    {
        SetTimescale(1);
        ThisCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (MergableLayerMask.HaveLayer(other.gameObject))
        {
            SetTimescale(SlowTime);
            ECOnSlowTimePosition.Invoke(transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (MergableLayerMask.HaveLayer(other.gameObject))
        {
            SetTimescale(1);
        }
    }

    private void SetTimescale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = Time.timeScale * FixedUpdateTime;
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
