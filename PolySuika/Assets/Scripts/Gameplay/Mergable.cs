using PrimeTween;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class Mergable : MonoBehaviour
{
    private int Tier = 0;
    private bool isMerging = false;
    private bool isImpacted = false;

    public event Action<int> EOnMergeTrigger;
    public event Action<Vector3> EOnMergePosition;
    public event Action<Mergable> EOnImpact;
    public event Action<Mergable> EOnMerging;
    public event Action<Mergable> EOnDisable;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isImpacted)
        {
            SetImpacted(true);
        }
        if (isMerging || Tier == TierManager.instance.GetMaxTier() - 1)
        {
            return;
        }
        // Check if the colliding object has a Mergable component
        Mergable otherMergable = collision.gameObject.GetComponent<Mergable>();
        if (otherMergable != null
            && otherMergable.GetTier() == Tier
            && otherMergable.isMerging == false)
        {
            SetIsMerging(true);
            otherMergable.SetIsMerging(true);
            otherMergable.GetComponent<Collider>().enabled = false;
            GetComponent<Collider>().enabled = false;
            Vector3 mergePosition = (otherMergable.transform.position + transform.position) / 2;
            TierManager.instance.SpawnAdvance(Tier + 1, mergePosition);
            EOnMergeTrigger?.Invoke(Tier);
            EOnMergePosition?.Invoke(mergePosition);

            // Should return this to pool
            TierManager.instance.ReturnMergable(otherMergable);
            TierManager.instance.ReturnMergable(this);
        }
    }

    public void SetTier(int tier) { Tier = tier; }
    public int GetTier() { return Tier; }
    public void SetImpacted(bool imp) 
    { 
        isImpacted = imp;
        if (isImpacted)
        {
            EOnImpact?.Invoke(this);
        }
    }
    public bool IsImpacted() { return isImpacted; }
    public bool IsMerging() { return isMerging; }

    public void SetIsMerging(bool merging) { 
        isMerging = merging;
        if (merging)
        {
            EOnMerging?.Invoke(this);
        }
    }

    void OnEnable()
    {
        isMerging = false;
        isImpacted = false;
    }

    private void OnDisable()
    {
        Tween.StopAll(transform);
        EOnDisable?.Invoke(this);
    }
}