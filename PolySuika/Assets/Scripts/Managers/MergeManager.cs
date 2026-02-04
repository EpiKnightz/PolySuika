using System.Collections.Generic;
using Lean.Pool;
using PrimeTween;
using Reflex.Core;
using Sortify;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

public class MergeManager : MonoBehaviour, IInstaller, IMergeHandler, ICooldown, IScaleIncrement
{
    [BetterHeader("Spawn Settings")]
    [SerializeField] private int MaxSpawnableTier = 2;
    [SerializeField] private int TierUpMergeCount = 5;
    [SerializeField] private float CooldownTime = 1f;
    [SerializeField] private Transform OffsetTransform;
    [SerializeField] private float MinSpawnHeight = 2f;
    [SerializeField] private float MaxSpawnHeight = 2.5f;
    [SerializeField] private float RandomSpawnAngle = 90f;
    [SerializeField] private Vector3 BaseSpawnPosition = new(0, 0, 1);

    [Header("Tier Increment")]
    [SerializeField] private float MassIncrement = 1;
    [SerializeField] private float TimeIncrement = 0.15f;

    [Header("Spawn Preview")]
    [SerializeField] private Vector3 SpawnNextPosition = new(-0.45f, 2.35f, -4);
    [SerializeField] private float SpawnNextScaleMultiplier = 0.6f;

    [Header("Variables")]
    [SerializeField] private float PopUpStartScale = 0.001f;
    [SerializeField] private HandleMergeRequestSO DefaultMergeHandler;

    [BetterHeader("Broadcast On")]
    public IntEventChannelSO ECOnClearTierEvent = null;
    public VectorEventChannelSO ECOnClearPosition = null;
    public ObjectArrayEventChannelSO ECOnRequestRefSpawn = null;
    public FloatEventChannelSO ECOnRefScaleChange = null;
    public VectorEventChannelSO ECOnSpawnPosition = null;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECActionButtonTriggered;
    public VoidEventChannelSO ECOnRestartTriggered;
    public IntEventChannelSO ECOnSetChange;
    public VoidEventChannelSO[] ECOnGameEndTriggerList;
    public LevelSetEventChannelSO ECOnLevelSetChange;
    public IntFloatEventChannelSO ECOnReachTargetTier;
    public VectorEventChannelSO ECOnClickPosition;

    // Privates
    private float LastSpawnTime = 0;
    private float ScaleIncrement = 0.35f;
    private GameObject[] TierPrefabs;
    private float BaseScale = 1f;
    private Mergable NextMergable;
    private HashSet<Mergable> CurrentMergableList = new();
    private int CurrentMaxSpawnableTier = 0;
    private int CurrentMergeCount = 0;
    private bool IsLevelDirty = true;
    private bool IsMergeEnable = true;


    private void OnEnable()
    {
        ECActionButtonTriggered.Sub(OnActionTriggered);
        ECOnSetChange.Sub(OnCurrentSetChanged);
        ECOnRestartTriggered.Sub(RemoveAllMergable);
        ECOnLevelSetChange.Sub(UpdateTierPrefabs);
        ECOnReachTargetTier.Sub(ClearWithEffect);
        ECOnClickPosition.Sub(OnSpawn);
    }

    private void OnDisable()
    {
        ECActionButtonTriggered.Unsub(OnActionTriggered);
        ECOnSetChange.Unsub(OnCurrentSetChanged);
        ECOnRestartTriggered.Unsub(RemoveAllMergable);
        ECOnLevelSetChange.Unsub(UpdateTierPrefabs);
        ECOnReachTargetTier.Unsub(ClearWithEffect);
        ECOnClickPosition.Unsub(OnSpawn);
    }

    private void OnCurrentSetChanged(int newIdx)
    {
        if (!IsLevelDirty)
        {
            // Send Restart event
            IsLevelDirty = true;
        }
    }

    private void OnActionTriggered()
    {
        if (IsLevelDirty)
        {
            SpawnNext();
            ECOnRequestRefSpawn.Invoke(TierPrefabs);
            IsLevelDirty = false;
        }
    }

    private void UpdateTierPrefabs(LevelSetSO levelSet)
    {
        TierPrefabs = levelSet.TierPrefabs;
        BaseScale = levelSet.BaseScale;
        ScaleIncrement = levelSet.ScaleIncrement;
        ECOnRefScaleChange.Invoke(levelSet.BaseScale * levelSet.RefScale);
    }

    public int GetMaxTier()
    {
        return TierPrefabs.Length;
    }

    // Spawn and show as preview
    private void SpawnNext()
    {
        var rndTier = Random.Range(0, CurrentMaxSpawnableTier + 1);

        if (TierPrefabs[rndTier] != null)
        {
            var clone = SpawnAdvance(rndTier, SpawnNextPosition + OffsetTransform.position, false, false);
            if (clone != null)
            {
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
    }

    public void OnSpawn(Vector3 offsetPosition)
    {
        if (NextMergable == null
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
        ECOnSpawnPosition.Invoke(offsetPosition);
        PoppingUp(NextMergable.gameObject, NextMergable.GetTier());
        SpawnNext();
    }

    public GameObject SpawnAdvance(int tier, Vector3 offsetPosition, bool isMerged = true, bool usePrefabZ = true)
    {
        if (tier < 0
            || tier >= GetMaxTier())
        {
            return null;
        }
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
        if (newObject.TryGetComponent<Mergable>(out var newMergable))
        {
            newMergable.SetTier(tier);
            newMergable.SetMass(1 + (tier * MassIncrement));
            newMergable.SetMergeRequestDelegate(ProcessMergingRequest);
            if (isMerged)
            {
                PoppingUp(newObject, tier);
                newMergable.SetImpacted(true);
                newMergable.EnablePhysic(true);
                CurrentMergableList.Add(newMergable);
            }
        }
        else
        {
#if UNITY_EDITOR            
            Debug.Log("Warning: Object don't have mergable script");
#endif            
        }
        return newObject;
    }

    private void ProcessMergingRequest(Mergable first, Mergable second)
    {
        if (!IsMergeEnable
            || !DefaultMergeHandler.ValidateRequest(first, second))
        {
            return;
        }
        ReturnMergable(first);
        ReturnMergable(second);

        DefaultMergeHandler.PreprocessRequest(first, second);
        SpawnAdvance(DefaultMergeHandler.ProcessTierRequest(first, second),
                    DefaultMergeHandler.ProcessPositionRequest(first, second));
        DefaultMergeHandler.PostprocessRequest(first, second);
        IncreaseMergeCount();
    }

    private void IncreaseMergeCount()
    {
        CurrentMergeCount++;
        if (CurrentMergeCount % TierUpMergeCount == 0
            && CurrentMaxSpawnableTier < MaxSpawnableTier)
        {
            CurrentMaxSpawnableTier++;
        }
    }

    private void ResetTierSettings()
    {
        CurrentMaxSpawnableTier = 0;
        CurrentMergeCount = 0;
        LastSpawnTime = 0;
        NextMergable = null;
        IsMergeEnable = true;
    }

    private void PoppingUp(GameObject gameObj, int tier)
    {
        gameObj.transform.localScale = Vector3.one * PopUpStartScale;
        // Use tween to ramp up the scale of the object to their tier size
        Tween.Scale(gameObj.transform,
            BaseScale + (tier * ScaleIncrement * BaseScale),
            0.75f + (tier * TimeIncrement),
            ease: Ease.OutCirc);
    }

    private void RemoveAllMergable()
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
        ResetTierSettings();
        if (!IsLevelDirty)
        {
            SpawnNext();
        }
    }

    //Maybe add the interval here as parameter, to reduce the reponsibility of TierManager
    private void ClearWithEffect(int tierExclusion = -1, float expolodeInterval = 0.1f)
    {
        IsMergeEnable = false;
        int count = 0;
        List<Mergable> exclusionList = new();
        foreach (var script in CurrentMergableList)
        {
            if (script.GetTier() == tierExclusion)
            {
                exclusionList.Add(script);
            }
            else
            {
                count++;
                Tween.Delay(expolodeInterval * count, () =>
                {
                    OnClearTierEvent(script.GetTier());
                    OnClearPosition(script.transform.position);
                    script.DRequestMerging.Unreg(ProcessMergingRequest);
                    LeanPool.Despawn(script.gameObject);
                });
            }
        }
        CurrentMergableList.Clear();
        if (expolodeInterval > 0)
        {
            if (exclusionList.Count > 0)
            {
                CurrentMergableList.AddRange(exclusionList);
            }
            Tween.Delay(expolodeInterval * (count + exclusionList.Count), () =>
            {
                OnClearTierEvent(GConst.CLEAR_FINISHED_VALUE);
            });
        }
        else
        {
            OnClearTierEvent(GConst.CLEAR_FINISHED_VALUE);
        }
    }

    private void ReturnMergable(Mergable script, bool removeFromList = true)
    {
        script.DRequestMerging.Unreg(ProcessMergingRequest);
        if (removeFromList)
        {
            CurrentMergableList.Remove(script);
        }
        LeanPool.Despawn(script.gameObject);
    }

    private void OnClearTierEvent(int tier)
    {
        ECOnClearTierEvent.Invoke(tier);
    }

    private void OnClearPosition(Vector3 pos)
    {
        ECOnClearPosition.Invoke(pos);
    }

    public void SetMergeHandler(HandleMergeRequestSO newMergeHandler)
    {
        DefaultMergeHandler = newMergeHandler;
    }

    public HandleMergeRequestSO GetMergeHandler()
    {
        return DefaultMergeHandler;
    }

    public void SetCooldownTime(float value)
    {
        CooldownTime = value;
    }

    public float GetCooldownTime()
    {
        return CooldownTime;
    }

    public void SetScaleIncrement(float value)
    {
        ScaleIncrement = value;
    }

    public float GetScaleIncrement()
    {
        return ScaleIncrement;
    }

    public void InstallBindings(ContainerBuilder builder)
    {
        builder.AddSingleton(this, typeof(IMergeHandler));
        builder.AddSingleton(this, typeof(ICooldown));
        builder.AddSingleton(this, typeof(IScaleIncrement));
    }
}