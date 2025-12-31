using Lean.Pool;
using PrimeTween;
using Reflex.Attributes;
using Reflex.Core;
using Sortify;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TierManager : MonoBehaviour, ITierManager, IInstaller
{
    // Dependencies
    [Inject] private readonly IDataManager DataManager;

    [BetterHeader("Spawn Settings")]
    public int MaxSpawnableTier = 2;
    public int TierUpMergeCount = 5;
    public float CooldownTime = 1f;
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
    public VoidEventChannelSO ECOnRestartTriggered = null;
    public IntEventChannelSO ECOnSetChange;
    public VoidEventChannelSO ECOnLoseTrigger;

    // Privates
    private float lastSpawnTime = 0;
    private float scaleIncrement = 0.35f;
    private GameObject[] TierPrefabs;
    private float baseScale = 1f;
    private Mergable spawnedTransform;
    private Mergable cachedTransform;
    private int chosenTier = -1;
    private HashSet<Mergable> CurrentMergableList = new();
    private HashSet<GameObject> CurrentRefList = new();
    private int currentMaxTier = 0;
    private int currentMergeCount = 0;
    private bool bIsLevelDirty = true;
    private bool bIsGameEnd = false;
    private const float PHYSIC_WAIT_TIME = 0.001f;


    public void InstallBindings(ContainerBuilder builder)
    {
        builder.AddSingleton(this, typeof(ITierManager));
    }

    private void OnEnable()
    {
        ECActionButtonTriggered.Sub(OnActionTriggered);
        ECOnSetChange.Sub(OnCurrentSetChanged);
        ECOnRestartTriggered.Sub(ClearBoard);
        ECOnLoseTrigger.Sub(OnGameEnd);
    }

    private void OnDisable()
    {
        ECActionButtonTriggered.Unsub(OnActionTriggered);
        ECOnSetChange.Unsub(OnCurrentSetChanged);
        ECOnRestartTriggered.Unsub(ClearBoard);
        ECOnLoseTrigger.Unsub(OnGameEnd);
    }

    private void OnCurrentSetChanged(int newIdx)
    {
        if (CurrentMergableList.Count > 0)
        {
            // Send Restart event
            bIsLevelDirty = true;
            ReturnTierRefs();
            // Manually trigger restart to clear the board
            ECOnRestartTriggered.Invoke();
        }
    }

    private void OnActionTriggered()
    {
        if (bIsLevelDirty)
        {
            UpdateTierPrefabs();
            SpawnNext();
            SpawnReferences();
            bIsLevelDirty = false;
        }
    }

    private void UpdateTierPrefabs()
    {
        TierPrefabs = DataManager.GetCurrentTierPrefabs();
        baseScale = DataManager.GetCurrentBaseScale();
        scaleIncrement = DataManager.GetCurrentScaleIncrement();
    }

    public int GetMaxTier()
    {
        return TierPrefabs.Length;
    }

    // Spawn and show as preview
    private void SpawnNext()
    {
        chosenTier = Random.Range(0, currentMaxTier + 1);

        if (TierPrefabs[chosenTier] != null)
        {
            var clone = SpawnAdvance(chosenTier, SpawnNextPosition, false, false);
            if (clone.TryGetComponent<Mergable>(out var mergable))
            {
                if (spawnedTransform == null)
                {
                    spawnedTransform = mergable;
                }
                else
                {
                    cachedTransform = mergable;
                }
                mergable.EnablePhysic(false);
                mergable.EnableShadow(false);
            }
            float scaleFactor = baseScale * SpawnNextScaleMultiplier;
            clone.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            clone.SetActive(true);
        }
    }

    public void OnClick(Vector3 offsetPosition)
    {
        if (bIsGameEnd
            || spawnedTransform == null
            || Time.timeSinceLevelLoad - lastSpawnTime < CooldownTime)
        {
            return;
        }
        else
        {
            lastSpawnTime = Time.timeSinceLevelLoad;
        }
        offsetPosition.y = Mathf.Clamp(offsetPosition.y, MinSpawnHeight, MaxSpawnHeight);
        spawnedTransform.transform.SetPositionAndRotation(offsetPosition,
            spawnedTransform.transform.rotation);
        spawnedTransform.EnableShadow(true);

        PoppingUp(spawnedTransform.gameObject, chosenTier);
        SpawnNext();
        Tween.Delay(PHYSIC_WAIT_TIME, ResetPhysic);
    }

    private void ResetPhysic()
    {
        spawnedTransform.EnablePhysic(true);

        spawnedTransform = cachedTransform;
        cachedTransform = null;
    }

    public GameObject SpawnAdvance(int Tier, Vector3 offsetPosition, bool isMerged = true, bool usePrefabZ = true)
    {
        var finalPosition = TierPrefabs[Tier].transform.position + offsetPosition;
        if (usePrefabZ)
        {
            finalPosition.z = TierPrefabs[Tier].transform.position.z + BaseSpawnPosition.z;
        }

        Vector3 eulerRotation = Vector3.zero;
        float RandomAngle = Random.Range(-RandomSpawnAngle, RandomSpawnAngle);
        var rigidbody = TierPrefabs[Tier].GetComponent<Rigidbody>();
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
        var finalRotation = TierPrefabs[Tier].transform.rotation * Quaternion.Euler(eulerRotation);
        GameObject NewObject = LeanPool.Spawn(TierPrefabs[Tier],
                                    finalPosition,
                                    finalRotation);
        var newRigid = NewObject.GetComponent<Rigidbody>();
        newRigid.mass = 1 + (Tier * MassIncrement);
        Mergable NewMergable = NewObject.GetComponent<Mergable>();
        NewMergable.SetTier(Tier);
        NewMergable.DRequestMerging.Reg(ProcessMergingRequest);
        CurrentMergableList.Add(NewMergable);
        if (isMerged)
        {
            PoppingUp(NewObject, Tier);
            // HAX: Popup = true mean this is coming from a merge
            NewMergable.SetImpacted(true);
            NewMergable.EnablePhysic(true);
        }
        return NewObject;
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

    private void IncreaseMergeCount(int Tier)
    {
        currentMergeCount++;
        if (currentMergeCount % TierUpMergeCount == 0
            && currentMaxTier < MaxSpawnableTier)
        {
            currentMaxTier++;
        }
    }

    private void OnMergeEvent(int Tier)
    {
        ECOnMergeEvent.Invoke(Tier);
    }

    private void OnMergePosition(Vector3 pos)
    {
        ECOnMergePosition.Invoke(pos);
    }

    public void ResetTier()
    {
        currentMaxTier = 0;
        currentMergeCount = 0;
        lastSpawnTime = 0;
        spawnedTransform = null;
        cachedTransform = null;
        bIsGameEnd = false;
    }

    public void PoppingUp(GameObject Object, int Tier)
    {
        Object.transform.localScale = new Vector3(PopUpStartScale, PopUpStartScale, PopUpStartScale);
        // Use tween to ramp up the scale of the object to their tier size
        Tween.Scale(Object.transform,
            baseScale + (Tier * scaleIncrement * baseScale),
            0.75f + (Tier * TimeIncrement),
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
            GameObject NewObject = Instantiate(TierPrefabs[i],
                                        finalPosition,
                                        finalRotation);
            if (NewObject.TryGetComponent<Mergable>(out var mergable))
            {
                mergable.EnablePhysic(false);
                mergable.EnableShadow(false);
                mergable.enabled = false;
            }
            NewObject.transform.localScale = 0.5f * baseScale * Vector3.one;
            NewObject.layer = 0;
            NewObject.transform.SetParent(transform, true);
            CurrentRefList.Add(NewObject);
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
        ResetTier();
        if (!bIsLevelDirty)
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
        bIsGameEnd = true;
    }
}