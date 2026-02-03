using System.Collections.Generic;
using Sortify;
using UnityEngine;

public class ReferencesSpawner : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private Vector3 SpawnRefMinMax = new(1.3f, 3.25f, -2);
    [SerializeField] private Transform OffsetTransform;

    [BetterHeader("Listen To")]
    public ObjectArrayEventChannelSO ECOnRequestRefSpawn;
    public FloatEventChannelSO ECOnRefScaleChange;
    public IntEventChannelSO ECOnSetChange;

    // Privates
    private HashSet<GameObject> CurrentRefList = new();
    private float RefScale = 1.0f;

    private void OnEnable()
    {
        ECOnRequestRefSpawn.Sub(SpawnReferences);
        ECOnRefScaleChange.Sub(SetRefScale);
        ECOnSetChange.Sub(OnCurrentSetChanged);
    }

    private void OnDisable()
    {
        ECOnRequestRefSpawn.Unsub(SpawnReferences);
        ECOnRefScaleChange.Unsub(SetRefScale);
        ECOnSetChange.Sub(OnCurrentSetChanged);
    }

    private void SetRefScale(float scale)
    {
        RefScale = scale;
    }

    // Show reference of all tiers at the top of the screen
    private void SpawnReferences(GameObject[] refPrefabArray)
    {
        float minX = -SpawnRefMinMax.x; //- 1.5f;
        float maxX = SpawnRefMinMax.x;  //1.5f;
        float XSpacing = (maxX - minX) / (refPrefabArray.Length - 1);

        for (int i = 0; i < refPrefabArray.Length; i++)
        {
            var offsetPos = new Vector3(minX + (i * XSpacing), SpawnRefMinMax.y, SpawnRefMinMax.z);
            var finalPosition = refPrefabArray[i].transform.position + offsetPos;
            var finalRotation = refPrefabArray[i].transform.rotation;
            GameObject newObject = Instantiate(refPrefabArray[i],
                                        finalPosition,
                                        finalRotation, OffsetTransform);
            if (newObject.TryGetComponent<Mergable>(out var mergable))
            {
                mergable.EnablePhysic(false);
                mergable.EnableShadow(false);
                mergable.enabled = false;
                if (mergable.transform.childCount > 0)
                {
                    for (int j = 0; j < mergable.transform.childCount; j++)
                    {
                        mergable.transform.GetChild(j).gameObject.layer = LayerMask.NameToLayer("Default");
                    }
                }
            }
            newObject.transform.localScale = RefScale * Vector3.one;
            newObject.layer = 0;
            CurrentRefList.Add(newObject);
        }
    }

    private void OnCurrentSetChanged(int newIdx)
    {
        ReturnTierRefs();
    }

    private void ReturnTierRefs()
    {
        foreach (var obj in CurrentRefList)
        {
            Destroy(obj); // This is so bad
        }
        CurrentRefList.Clear();
    }
}
