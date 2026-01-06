using Lean.Pool;
using PrimeTween;
using Sortify;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TierManager : MonoBehaviour
{
    [BetterHeader("Spawn Settings")]
    public int MaxSpawnableTier = 2;
    public int TierUpMergeCount = 5;
    public float CooldownTime = 1f;
    public Transform OffsetTransform;
    public float MinSpawnHeight = 2f;
    public float MaxSpawnHeight = 2.5f;
    public float RandomSpawnAngle = 90f;
    public Vector3 BaseSpawnPosition = new(0, 0, 1);
    public float PopUpStartScale = 0.001f;

    [Header("Tier Increment")]
    public float MassIncrement = 1;
    public float TimeIncrement = 0.15f;

    [Header("Spawn Preview")]
    public Vector3 SpawnNextPosition = new(-0.45f, 2.35f, -4);
    public float SpawnNextScaleMultiplier = 0.6f;
    public Vector3 SpawnRefMinMax = new(1.35f, 3.25f, -2);

    [BetterHeader("Broadcast On")]
    public IntEventChannelSO ECOnMergeEvent = null;
    public VectorEventChannelSO ECOnMergePosition = null;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECActionButtonTriggered;
    public VoidEventChannelSO ECOnRestartTriggered;
    public IntEventChannelSO ECOnSetChange;
    public VoidEventChannelSO ECOnLoseTrigger;
    public LevelSetEventChannelSO ECOnLevelSetChange;

    // Privates
    private float LastSpawnTime = 0;
    private float ScaleIncrement = 0.35f;
    private GameObject[] TierPrefabs;
    private float BaseScale = 1f;
    private Mergable NextMergable;
    private HashSet<Mergable> CurrentMergableList = new();
    private HashSet<GameObject> CurrentRefList = new();
    private int CurrentMaxTier = 0;
    private int CurrentMergeCount = 0;
    private bool IsLevelDirty = true;
    private bool IsGameEnd = false;

    private void OnEnable()
    {
        ECActionButtonTriggered.Sub(OnActionTriggered);
        ECOnSetChange.Sub(OnCurrentSetChanged);
        ECOnRestartTriggered.Sub(ClearBoard);
        ECOnLoseTrigger.Sub(OnGameEnd);
        ECOnLevelSetChange.Sub(UpdateTierPrefabs);
    }

    private void OnDisable()
    {
        ECActionButtonTriggered.Unsub(OnActionTriggered);
        ECOnSetChange.Unsub(OnCurrentSetChanged);
        ECOnRestartTriggered.Unsub(ClearBoard);
        ECOnLoseTrigger.Unsub(OnGameEnd);
        ECOnLevelSetChange.Unsub(UpdateTierPrefabs);
    }

    private void OnCurrentSetChanged(int newIdx)
    {
        if (!IsLevelDirty)
        {
            // Send Restart event
            IsLevelDirty = true;
            ReturnTierRefs();
        }
    }

    private void OnActionTriggered()
    {
        if (IsLevelDirty)
        {
            SpawnNext();
            SpawnReferences();
            IsLevelDirty = false;
        }
    }

    private void UpdateTierPrefabs(LevelSet levelSet)
    {
        TierPrefabs = levelSet.TierPrefabs;
        BaseScale = levelSet.BaseScale;
        ScaleIncrement = levelSet.ScaleIncrement;
    }

    public int GetMaxTier()
    {
        return TierPrefabs.Length;
    }

    // Spawn and show as preview
    private void SpawnNext()
    {
        var rndTier = Random.Range(0, CurrentMaxTier + 1);

        if (TierPrefabs[rndTier] != null)
        {
            var clone = SpawnAdvance(rndTier, SpawnNextPosition + OffsetTransform.position, false, false);
            if (clone.TryGetComponent<Mergable>(out var mergable))
            {
                NextMergable = mergable;
                mergable.EnablePhysic(false);
                mergable.EnableShadow(false);
            }
            float scaleFactor = BaseScale * SpawnNextScaleMultiplier;
            clone.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            clone.SetActive(true);
        }
    }

    public void OnClick(Vector3 offsetPosition)
    {
        if (IsGameEnd
            || NextMergable == null
            || Time.timeSinceLevelLoad - LastSpawnTime < CooldownTime)
        {
            return;
        }
        LastSpawnTime = Time.timeSinceLevelLoad;

        offsetPosition.y = Mathf.Clamp(offsetPosition.y, OffsetTransform.position.y + MinSpawnHeight, OffsetTransform.position.y + MaxSpawnHeight);
        NextMergable.transform.SetParent(null);
        NextMergable.transform.position = offsetPosition;
        NextMergable.EnableShadow(true);
        NextMergable.EnablePhysic(true);
        CurrentMergableList.Add(NextMergable);

        PoppingUp(NextMergable.gameObject, NextMergable.GetTier());
        SpawnNext();
    }

    public GameObject SpawnAdvance(int tier, Vector3 offsetPosition, bool isMerged = true, bool usePrefabZ = true)
    {
        var finalPosition = TierPrefabs[tier].transform.position + offsetPosition;
        if (usePrefabZ)
        {
            finalPosition.z = TierPrefabs[tier].transform.position.z + BaseSpawnPosition.z;
        }

        Vector3 eulerRotation = Vector3.zero;
        float RandomAngle = Random.Range(-RandomSpawnAngle, RandomSpawnAngle);
        if (TierPrefabs[tier].TryGetComponent<Rigidbody>(out var rigidbody))
        {
            if ((rigidbody.constraints & RigidbodyConstraints.FreezeRotationZ)
                != RigidbodyConstraints.FreezeRotationZ)
            {
                eulerRotation.z = RandomAngle;
            }
            else if ((rigidbody.constraints & RigidbodyConstraints.FreezeRotationX)
                != RigidbodyConstraints.FreezeRotationX)
            {
                eulerRotation.x = RandomAngle;
            }
            else
            {
                eulerRotation.y = RandomAngle;
            }
        }
        var finalRotation = TierPrefabs[tier].transform.rotation * Quaternion.Euler(eulerRotation);
        var newObject = isMerged ? LeanPool.Spawn(TierPrefabs[tier], finalPosition, finalRotation)
                                    : LeanPool.Spawn(TierPrefabs[tier], finalPosition, finalRotation, OffsetTransform);
        Mergable newMergable = newObject.GetComponent<Mergable>();
        newMergable.SetTier(tier);
        newMergable.SetMass(1 + (tier * MassIncrement));
        newMergable.DRequestMerging.Reg(ProcessMergingRequest);
        if (isMerged)
        {
            PoppingUp(newObject, tier);
            newMergable.SetImpacted(true);
            newMergable.EnablePhysic(true);
            CurrentMergableList.Add(newMergable);
        }
        return newObject;
    }

    private void ProcessMergingRequest(Mergable first, Mergable second)
    {
        if (first.GetTier() == GetMaxTier() - 1)
        {
            return;
        }
        second.SetIsMerging(true);
        second.EnablePhysic(false);
        first.EnablePhysic(false);
        first.SetIsMerging(true);

        Vector3 mergePosition = (first.transform.position + second.transform.position) / 2;
        int firstTier = first.GetTier();
        SpawnAdvance(firstTier + 1, mergePosition);
        OnMergeEvent(firstTier);
        IncreaseMergeCount(firstTier);
        OnMergePosition(mergePosition);

        ReturnMergable(first);
        ReturnMergable(second);
    }

    private void IncreaseMergeCount(int tier)
    {
        CurrentMergeCount++;
        if (CurrentMergeCount % TierUpMergeCount == 0
            && CurrentMaxTier < MaxSpawnableTier)
        {
            CurrentMaxTier++;
        }
    }

    private void OnMergeEvent(int tier)
    {
        ECOnMergeEvent.Invoke(tier);
    }

    private void OnMergePosition(Vector3 pos)
    {
        ECOnMergePosition.Invoke(pos);
    }

    public void ResetTier()
    {
        CurrentMaxTier = 0;
        CurrentMergeCount = 0;
        LastSpawnTime = 0;
        NextMergable = null;
        IsGameEnd = false;
    }

    public void PoppingUp(GameObject gameObj, int tier)
    {
        gameObj.transform.localScale = new Vector3(PopUpStartScale, PopUpStartScale, PopUpStartScale);
        // Use tween to ramp up the scale of the object to their tier size
        Tween.Scale(gameObj.transform,
            BaseScale + (tier * ScaleIncrement * BaseScale),
            0.75f + (tier * TimeIncrement),
            ease: Ease.OutCirc);
    }

    // Show reference of all tiers at the top of the screen
    private void SpawnReferences()
    {
        float minX = -SpawnRefMinMax.x; //- 1.5f;
        float maxX = SpawnRefMinMax.x;  //1.5f;
        float XSpacing = (maxX - minX) / (TierPrefabs.Length - 1);

        for (int i = 0; i < TierPrefabs.Length; i++)
        {
            var offsetPos = new Vector3(minX + (i * XSpacing), SpawnRefMinMax.y, SpawnRefMinMax.z);
            var finalPosition = TierPrefabs[i].transform.position + offsetPos;
            var finalRotation = TierPrefabs[i].transform.rotation;
            GameObject newObject = Instantiate(TierPrefabs[i],
                                        finalPosition,
                                        finalRotation, OffsetTransform);
            if (newObject.TryGetComponent<Mergable>(out var mergable))
            {
                mergable.EnablePhysic(false);
                mergable.EnableShadow(false);
                mergable.enabled = false;
            }
            newObject.transform.localScale = 0.5f * BaseScale * Vector3.one;
            newObject.layer = 0;
            CurrentRefList.Add(newObject);
        }
    }


    private void ClearBoard()
    {
        foreach (var script in CurrentMergableList)
        {
            LeanPool.Detach(script);
            Destroy(script.gameObject);
        }
        CurrentMergableList.Clear();
        if (NextMergable != null)
        {
            LeanPool.Detach(NextMergable);
            Destroy(NextMergable.gameObject);
        }
        ResetTier();
        if (!IsLevelDirty)
        {
            SpawnNext();
        }
    }

    public void ReturnMergable(Mergable script, bool removeFromList = true)
    {
        script.DRequestMerging.Unreg(ProcessMergingRequest);
        if (removeFromList)
        {
            CurrentMergableList.Remove(script);
        }
        LeanPool.Despawn(script.gameObject);
    }

    public void ReturnTierRefs()
    {
        foreach (var obj in CurrentRefList)
        {
            Destroy(obj); // This is so bad
        }
        CurrentRefList.Clear();
    }

    private void OnGameEnd()
    {
        IsGameEnd = true;
    }
}