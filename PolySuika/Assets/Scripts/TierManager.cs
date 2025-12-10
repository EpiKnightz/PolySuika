using PrimeTween;
using Unity.Mathematics;
using UnityEngine;
using WanzyeeStudio;
using System.Collections;
using TMPro;
using FirstGearGames.SmoothCameraShaker;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;
using Utilities;


public class TierManager : BaseSingleton<TierManager>
{
    public GameObject[] TierPrefabs;
    public int maxSpawnableTier = 3;
    public int tierUpMergeCount = 5;
    private int currentMaxTier = 0;
    private int currentMergeCount = 0;

    public float cooldownTime = 0.5f;
    private float lastSpawnTime = 0;
    public float minSpawnHeight = 1.35f;
    public float maxSpawnHeight = 2f;

    public float massIncrement = 1;
    public float timeIncrement = 0.15f;
    public float scaleIncrement = 0.35f;

    public Vector3 spawnPosition;

    private Transform spawnedTransform;
    private Transform cachedTransform;
    private int chosenTier = -1;

    private event IntEvent EUIEvents;


    // FX
    public GameObject MergeVFXPrefab;
    public AudioClip MergeSFX;
    public ShakeData[] MergeCamFX;
    private ChromaticAberration chromaticAberration;

    private void Start()
    {
        SpawnNext();
        SpawnReferences();
        VolumeProfile profile = GameObject.Find("Post Processing").GetComponent<Volume>().profile;
        if (profile != null)
        {
            profile.TryGet(out chromaticAberration);
        }
        UIScoreText scoreText = FindAnyObjectByType<UIScoreText>();
        if (scoreText != null)
        {
            EUIEvents += scoreText.UpdateScoreWhenMerge;
        }
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            EUIEvents += scoreManager.OnMergeEvent;
        }
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
            var clone = SpawnAdvance(chosenTier, spawnPosition, false);
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
            clone.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            clone.SetActive(true);
        }
    }

    public void OnClick(Vector3 offsetPosition)
    {
        if (spawnedTransform == null
            || Time.timeSinceLevelLoad - lastSpawnTime < cooldownTime)
        {
            return;
        }
        else
        {
            lastSpawnTime = Time.timeSinceLevelLoad;
        }
        offsetPosition.y = math.clamp(offsetPosition.y, minSpawnHeight, maxSpawnHeight);
        spawnedTransform.SetPositionAndRotation(offsetPosition,
            spawnedTransform.rotation);
        spawnedTransform.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
        PoppingUp(spawnedTransform.gameObject, chosenTier);

        var coroutine = WaitPhysic(0.1f);
        StartCoroutine(coroutine);

        SpawnNext();
    }

    // suspend execution for waitTime seconds
    private IEnumerator WaitPhysic(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        spawnedTransform.GetComponent<Collider>().enabled = true;
        spawnedTransform.GetComponent<Rigidbody>().isKinematic = false;

        spawnedTransform = cachedTransform;
        cachedTransform = null;
    }

    public GameObject SpawnAdvance(int Tier, Vector3 offsetPosition, bool popup = true)
    {
        var finalPosition = TierPrefabs[Tier].transform.position + offsetPosition;
        Vector3 eulerRotation = new Vector3(0, Random.Range(-30.0f, 30.0f), 0);
        if ((TierPrefabs[Tier].GetComponent<Rigidbody>().constraints & RigidbodyConstraints.FreezeRotationY)
            == RigidbodyConstraints.FreezeRotationY)
        {
            eulerRotation.z = eulerRotation.y;
            eulerRotation.y = 0;
        }
        var finalRotation = TierPrefabs[Tier].transform.rotation * Quaternion.Euler(eulerRotation);
        finalPosition.z = TierPrefabs[Tier].transform.position.z;
        //finalRotation.SetEulerRotation(0,0,offsetZ);
        GameObject NewObject = Instantiate(TierPrefabs[Tier],
                                    finalPosition,
                                    finalRotation);
        NewObject.GetComponent<Rigidbody>().mass = 1 + (Tier * massIncrement);
        Mergable NewMergable = NewObject.GetComponent<Mergable>();
        NewMergable.SetTier(Tier);
        NewMergable.EOnMergeTrigger += IncreaseMergeCount;
        NewMergable.EOnMergeTrigger += OnMergeEvent;
        NewMergable.EOnMergeTrigger += CameraShakeFX;
        NewMergable.EOnMergePosition += SpawnVFX;
        NewMergable.EOnMergePosition += PlaySFX;
        if (popup)
        {
            PoppingUp(NewObject, Tier);
        }
        return NewObject;
    }

    public void IncreaseMergeCount(int Tier)
    {
        currentMergeCount++;
        if (currentMergeCount % tierUpMergeCount == 0
            && currentMaxTier < maxSpawnableTier)
        {
            currentMaxTier++;
        }
    }

    public void OnMergeEvent(int Tier)
    {
        EUIEvents?.Invoke(Tier);
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
        if (MergeSFX != null)
        {
            AudioSource.PlayClipAtPoint(MergeSFX, position);
        }
    }

    public void PoppingUp(GameObject Object, int Tier)
    {
        Object.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        // Use tween to ramp up the scale of the object to their tier size
        Tween.Scale(Object.transform,
            (1f + Tier * scaleIncrement),
            (0.75f + Tier * timeIncrement),
            ease: Ease.OutCirc);
    }

    // Show reference of all tiers at the top of the screen
    private void SpawnReferences()
    {
        float minX = -1.5f;
        float maxX = 1.5f;
        float refY = 3.75f;
        float XSpacing = (maxX - minX) / (TierPrefabs.Length - 1);

        for (int i = 0; i < TierPrefabs.Length; i++)
        {
            var offsetPos = new Vector3(minX + i * XSpacing, refY, 0);
            var finalPosition = TierPrefabs[i].transform.position + offsetPos;
            var finalRotation = TierPrefabs[i].transform.rotation;
            //finalRotation.SetEulerRotation(0,0,offsetZ);
            GameObject NewObject = Instantiate(TierPrefabs[i],
                                        finalPosition,
                                        finalRotation);
            NewObject.GetComponent<Collider>().enabled = false;
            NewObject.GetComponent<Rigidbody>().isKinematic = true;
            NewObject.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            NewObject.transform.localScale = Vector3.one * 0.5f;
        }
    }
}