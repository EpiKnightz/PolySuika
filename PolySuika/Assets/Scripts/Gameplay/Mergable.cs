using PrimeTween;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(TrailRenderer))]
public class Mergable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] private Collider[] ColliderList;
    [SerializeField] private Renderer Renderer;
    [SerializeField] private TrailRenderer TrailRenderer;

    [Header("Variable")]
    [SerializeField] private string ImpactedTag;

    private int Tier = 0;
    private bool isMerging = false;
    private bool isImpacted = false;

    public event Action<Mergable> EOnImpact;
    public event Action<Mergable> EOnMerging;
    public event Action<Mergable> EOnDisable;
    public SingleDelegate<Mergable, Mergable> DRequestMerging = new();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == gameObject)
        {
            return;
        }
        if (!isImpacted)
        {
            if (collision.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            {
                SetImpacted(true);
            }
        }
        if (collision.gameObject.layer != gameObject.layer
            || isMerging)
        {
            return;
        }
        // Check if the colliding object has a Mergable component
        if (collision.gameObject.TryGetComponent(out Mergable otherMergable))
        {
            if (otherMergable.GetTier() == Tier
                && !otherMergable.IsMerging()
                && gameObject.GetInstanceID() < collision.gameObject.GetInstanceID())
            {
                DRequestMerging.Invoke(this, otherMergable);
            }
        }

    }

    public void EnablePhysic(bool isEnable)
    {
        Rigidbody.isKinematic = !isEnable;
        foreach (var collider in ColliderList)
        {
            collider.enabled = isEnable;
        }
    }

    public void EnableShadow(bool isEnable)
    {
        Renderer.receiveShadows = isEnable;
        Renderer.shadowCastingMode = isEnable ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
        TrailRenderer.enabled = isEnable;
        if (!isEnable)
        {
            TrailRenderer.Clear();
        }
    }

    public void SetMergeRequestDelegate(UnityAction<Mergable, Mergable> dele)
    {
        DRequestMerging.Reg(dele);
    }
    public void SetTier(int tier) { Tier = tier; }
    public int GetTier() { return Tier; }
    public void SetImpacted(bool imp)
    {
        isImpacted = imp;
        if (isImpacted)
        {
            gameObject.tag = ImpactedTag.ToString();
            EOnImpact?.Invoke(this);
        }
        else
        {
            gameObject.tag = "Untagged";
        }
    }
    public bool IsImpacted() { return isImpacted; }
    public bool IsMerging() { return isMerging; }

    public void SetMass(float mass)
    {
        Rigidbody.mass = mass;
    }

    public void SetIsMerging(bool merging)
    {
        isMerging = merging;
        if (merging)
        {
            EOnMerging?.Invoke(this);
        }
    }

    private void OnEnable()
    {
        SetIsMerging(false);
        SetImpacted(false);
    }

    private void OnDisable()
    {
        Tween.StopAll(transform);
        EOnDisable?.Invoke(this);
        EOnImpact = null;
        EOnMerging = null;
    }

    private void Awake()
    {
        Rigidbody.centerOfMass = Vector3.zero;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Rigidbody == null)
            Rigidbody = GetComponent<Rigidbody>();
        if (ColliderList == null || ColliderList.Length == 0)
            ColliderList = GetComponents<Collider>();
        if (Renderer == null)
            Renderer = GetComponent<Renderer>();
        if (TrailRenderer == null)
        {
            TrailRenderer = GetComponent<TrailRenderer>();
            if (TrailRenderer.startColor == Color.white.WithAlpha(0)
                && TrailRenderer.endColor == Color.white.WithAlpha(0))
            {
                TrailRenderer.startColor = Renderer.sharedMaterial.color.WithAlpha(0);
                TrailRenderer.endColor = Renderer.sharedMaterial.color.WithAlpha(0);
            }
        }
        ImpactedTag ??= "Impacted";
    }
#endif
}