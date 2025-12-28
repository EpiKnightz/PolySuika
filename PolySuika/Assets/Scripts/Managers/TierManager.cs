using FirstGearGames.SmoothCameraShaker;
using FMODUnity;
using Lean.Common;
using Lean.Pool;
using PrimeTween;
using Solo.MOST_IN_ONE;
using Sortify;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Utilities;
using WanzyeeStudio;
using Random = UnityEngine.Random;

public class TierManager : BaseSingleton<TierManager>
{ 
    [BetterHeader("Spawn Settings")]
    public int MaxSpawnableTier = 2;
    public int TierUpMergeCount = 5;
    public float CooldownTime = 1f;    
    public float MinSpawnHeight = 2f;
    public float MaxSpawnHeight = 2.5f;
    public float RandomSpawnAngle = 90f;
    public Vector3 BaseSpawnPosition = new Vector3(0, 0, 1);

    [Header("Tier Increment")]
    public float MassIncrement = 1;
    public float TimeIncrement = 0.15f;    

    [Header("Spawn Preview")]
    public Vector3 SpawnNextPosition = new Vector3(-0.45f, 2.35f, -4);
    public float SpawnNextScaleMultiplier = 0.6f;
    public Vector3 SpawnRefMinMax = new Vector3(1.35f, 3.25f, -2);

    [BetterHeader("FX")]
    public GameObject MergeVFXPrefab;
    public EventReference MergeSFX;
    public ShakeData[] MergeCamFX;
    public float PopUpStartScale = 0.001f;
    private ChromaticAberration chromaticAberration;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECActionButtonTriggered;
    public VoidEventChannelSO ECOnRestartTriggered;
    public IntEventChannelSO ECOnSetChange;

    // Events
    private event IntEvent EMergeTierEvent;
    private GetFloatEvent DGetCurrentBaseScale;
    private GetFloatEvent DGetCurrentScaleIncrement;
    private GetObjectsEvent DGetCurrentTierPrefabs;
    private VoidEvent DTriggerRestartManually;

    // Privates
    private float lastSpawnTime = 0;
    private float scaleIncrement = 0.35f;
    private GameObject[] TierPrefabs;
    private float baseScale = 1f;
    private Transform spawnedTransform;
    private Transform cachedTransform;
    private int chosenTier = -1;
    private HashSet<Mergable> CurrentMergableList = new();
    private HashSet<GameObject> CurrentRefList = new();
    private int currentMaxTier = 0;
    private int currentMergeCount = 0;
    private bool bIsLevelDirty = true;
    private bool bIsGameEnd = false;
    private const float PHYSIC_WAIT_TIME = 0.001f;

    private void Start()
    {        
        DataManager dataMan = FindAnyObjectByType<DataManager>();
        if (dataMan != null)
        {
            DGetCurrentTierPrefabs += dataMan.GetCurrentTierPrefabs;
            DGetCurrentBaseScale += dataMan.GetCurrentBaseScale;
            DGetCurrentScaleIncrement += dataMan.GetCurrentScaleIncrement;
        } else
        {           
            print("TierManager: DataManager not found in scene!");
            return;
        }
        var profile = GameObject.Find("Post Processing").GetComponent<Volume>().profile;
        if (profile != null)
        {
            profile.TryGet(out chromaticAberration);
        }
        var scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            EMergeTierEvent += scoreManager.OnMergeEvent;
        }
        var uIRestartButton = FindAnyObjectByType<UIRestartButton>();
        if (uIRestartButton != null)
        {
            DTriggerRestartManually += uIRestartButton.OnClick;
        }
        var checkFull = FindAnyObjectByType<CheckFull>();
        if (checkFull != null)
        {
            checkFull.EOnLoseTrigger += OnGameEnd;
        }
    }

    private void OnEnable()
    {
        ECActionButtonTriggered.Sub(OnActionTriggered);
        ECOnSetChange.Sub(OnCurrentSetChanged);
        ECOnRestartTriggered.Sub(ClearBoard);
    }

    private void OnDisable()
    {
        ECActionButtonTriggered.UnSub(OnActionTriggered);
        ECOnSetChange.Unsub(OnCurrentSetChanged);
        ECOnRestartTriggered.UnSub(ClearBoard);
    }

    public void OnCurrentSetChanged(int newIdx)
    {
        if (CurrentMergableList.Count > 0)
        {
            // Send Restart event
            bIsLevelDirty = true;
            ReturnTierRefs();
            DTriggerRestartManually?.Invoke();       
        }
    }

    public void OnActionTriggered()
    {
        if (bIsLevelDirty)
        {
            UpdateTierPrefabs();
            SpawnNext();
            SpawnReferences();
            bIsLevelDirty = false;
        }
    }

    public void UpdateTierPrefabs()
    {
        TierPrefabs = DGetCurrentTierPrefabs?.Invoke();
        baseScale = DGetCurrentBaseScale.Invoke();
        scaleIncrement = DGetCurrentScaleIncrement.Invoke();
    }

    public int GetMaxTier()
    {
        return TierPrefabs.Length;
    }

    // Spawn and show as preview
    public void SpawnNext()
    {
        chosenTier = Random.Range(0, currentMaxTier + 1);

        if (TierPrefabs[chosenTier] != null)
        {
            var clone = SpawnAdvance(chosenTier, SpawnNextPosition, false, false);
            if (spawnedTransform == null)
            {
                spawnedTransform = clone.transform;
            }
            else
            {
                cachedTransform = clone.transform;
            }
            clone.GetComponent<Collider>().enabled = false;
            clone.GetComponent<Rigidbody>().isKinematic = true;
            var renderer = clone.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.shadowCastingMode = ShadowCastingMode.Off;
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
        offsetPosition.y = math.clamp(offsetPosition.y, MinSpawnHeight, MaxSpawnHeight);
        spawnedTransform.SetPositionAndRotation(offsetPosition,
            spawnedTransform.rotation);
        var renderer = spawnedTransform.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.shadowCastingMode = ShadowCastingMode.On;
        }

        PoppingUp(spawnedTransform.gameObject, chosenTier);        
        // MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.SoftImpact);
        SpawnNext();
        Tween.Delay(PHYSIC_WAIT_TIME, ResetPhysic);
    }

    void ResetPhysic()
    {
        spawnedTransform.GetComponent<Collider>().enabled = true;
        spawnedTransform.GetComponent<Rigidbody>().isKinematic = false;

        spawnedTransform = cachedTransform;
        cachedTransform = null;
    }

    public GameObject SpawnAdvance(int Tier, Vector3 offsetPosition, bool popup = true, bool usePrefabZ = true)
    {
        var finalPosition = TierPrefabs[Tier].transform.position + offsetPosition;
        if (usePrefabZ)
        {
            finalPosition.z = TierPrefabs[Tier].transform.position.z + BaseSpawnPosition.z;
        }

        Vector3 eulerRotation = Vector3.zero;
        float RandomAngle = Random.Range(-RandomSpawnAngle, RandomSpawnAngle);
        if ((TierPrefabs[Tier].GetComponent<Rigidbody>().constraints & RigidbodyConstraints.FreezeRotationZ)
            != RigidbodyConstraints.FreezeRotationZ)
        {
            eulerRotation.z = RandomAngle;
        }else if ((TierPrefabs[Tier].GetComponent<Rigidbody>().constraints & RigidbodyConstraints.FreezeRotationX)
            != RigidbodyConstraints.FreezeRotationX)
        {
            eulerRotation.x = RandomAngle;
        }else
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
        NewMergable.EOnMergeTrigger += IncreaseMergeCount;
        NewMergable.EOnMergeTrigger += OnMergeEvent;
        NewMergable.EOnMergeTrigger += CameraShakeFX;
        NewMergable.EOnMergeTrigger += HapticFeedback;
        NewMergable.EOnMergePosition += SpawnVFX;
        NewMergable.EOnMergePosition += PlaySFX;
        CurrentMergableList.Add(NewMergable);
        if (popup)
        {
            PoppingUp(NewObject, Tier);
            // HAX: Popup = true mean this is coming from a merge
            NewMergable.SetImpacted(true);
            newRigid.isKinematic = false;
            NewObject.GetComponent<Collider>().enabled = true;
        }
        return NewObject;
    }

    public void IncreaseMergeCount(int Tier)
    {
        currentMergeCount++;
        if (currentMergeCount % TierUpMergeCount == 0
            && currentMaxTier < MaxSpawnableTier)
        {
            currentMaxTier++;
        }
    }

    public void OnMergeEvent(int Tier)
    {
        EMergeTierEvent?.Invoke(Tier);
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

    public void CameraShakeFX(int Tier)
    {
        // Might increase the shake intensity based on Tier?
        float intensity = 0.3f + 0.1f * Tier;
        int shakeIntensityIdx = 0;
        if (MergeCamFX.Length > 0)
        {
            if (MergeCamFX.Length == 3)
            {
                shakeIntensityIdx = Tier < GConst.TIER_RANK_1 ? 0 : Tier < GConst.TIER_RANK_2 ? 1 : 2;
            }
            CameraShakerHandler.Shake(MergeCamFX[shakeIntensityIdx]);
        }
        if (chromaticAberration != null)
        {
            Tween.Custom(0f, intensity, 0.125f, cycles: 2, cycleMode: CycleMode.Rewind, ease: Ease.InOutSine, onValueChange: newVal => {
                chromaticAberration.intensity.Override(newVal);
            });
        }
    }

    public void HapticFeedback(int Tier)
    {
        if (Tier < GConst.TIER_RANK_1)
        {
            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.LightImpact);
        }
        else if (Tier < GConst.TIER_RANK_2)
        {
            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.MediumImpact);
        }
        else if (Tier < GConst.TIER_RANK_3)
        {
            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.HeavyImpact);
        }
        else
        {
            MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.Failure);
        }
    }

    public void SpawnVFX(Vector3 position)
    {
        if (MergeVFXPrefab != null)
        {
            var vfx = Instantiate(MergeVFXPrefab,
                position,
                Quaternion.identity);
            Destroy(vfx, 2f);
        }
    }

    public void PlaySFX(Vector3 position)
    {
        RuntimeManager.PlayOneShot(MergeSFX, position);
    }

    public void PoppingUp(GameObject Object, int Tier)
    {
        Object.transform.localScale = new Vector3(PopUpStartScale, PopUpStartScale, PopUpStartScale);
        // Use tween to ramp up the scale of the object to their tier size
        Tween.Scale(Object.transform,
            (baseScale + Tier * scaleIncrement * baseScale),
            (0.75f + Tier * TimeIncrement),
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
            var offsetPos = new Vector3(minX + i * XSpacing, SpawnRefMinMax.y, SpawnRefMinMax.z);
            var finalPosition = TierPrefabs[i].transform.position + offsetPos;
            var finalRotation = TierPrefabs[i].transform.rotation;
            //finalRotation.SetEulerRotation(0,0,offsetZ);
            GameObject NewObject = Instantiate(TierPrefabs[i],
                                        finalPosition,
                                        finalRotation);            
            NewObject.GetComponent<Collider>().enabled = false;
            NewObject.GetComponent<Rigidbody>().isKinematic = true;
            var renderer = NewObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.shadowCastingMode = ShadowCastingMode.Off;
            }
            NewObject.GetComponent<Mergable>().enabled = false;
            NewObject.transform.localScale = Vector3.one * baseScale * 0.5f;
            NewObject.layer = 0;
            NewObject.transform.SetParent(transform, true);
            CurrentRefList.Add(NewObject);
        }
    }


    public void ClearBoard()
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

    // Psuedo section, using pool later
    public void ReturnMergable(Mergable script, bool removeFromList = true)
    {
        script.EOnMergeTrigger -= IncreaseMergeCount;
        script.EOnMergeTrigger -= OnMergeEvent;
        script.EOnMergeTrigger -= CameraShakeFX;
        script.EOnMergeTrigger -= HapticFeedback;
        script.EOnMergePosition -= SpawnVFX;
        script.EOnMergePosition -= PlaySFX;
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

    public void OnGameEnd()
    {
        bIsGameEnd = true;
    }
}