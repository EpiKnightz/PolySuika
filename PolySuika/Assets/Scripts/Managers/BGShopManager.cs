using Lean.Pool;
using PrimeTween;
using Sortify;
using UnityEngine;

public class BGShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform BGShopParent;

    [BetterHeader("Variables")]
    public Vector3 MainPosition = Vector3.zero;
    public float RightSpawnPosition = 60f;
    public float AnimDuration = 1f;

    [BetterHeader("Listen To")]
    public IntEventChannelSO ECOnSetIndexOffset;
    public LevelSetEventChannelSO ECOnLevelSetChange;

    // Private
    private Transform CurrentBGShop;
    private Transform OldBGShop;
    private float AnimDirection = 0;

    private void OnEnable()
    {
        ECOnSetIndexOffset.Sub(ChangeAnimDirection);
        ECOnLevelSetChange.Sub(ChangeBackgroundShop);
    }

    private void OnDisable()
    {
        ECOnSetIndexOffset.Unsub(ChangeAnimDirection);
        ECOnLevelSetChange.Unsub(ChangeBackgroundShop);
    }

    private void ChangeAnimDirection(int offset)
    {
        AnimDirection = RightSpawnPosition * offset;
    }


    private void ChangeBackgroundShop(LevelSet setData)
    {
        if (CurrentBGShop != null)
        {
            OldBGShop = CurrentBGShop;
            SpawnBGShop(setData.ShopPrefab, Vector3.right * AnimDirection);
            OldBGShop.gameObject.SetActive(true);
            CurrentBGShop.gameObject.SetActive(true);
            Tween.PositionX(OldBGShop, -AnimDirection, AnimDuration, Ease.OutBack);
            Tween.PositionX(CurrentBGShop, 0, AnimDuration, Ease.OutBack);
            Tween.Delay(AnimDuration, DeactiveOldShop);
        }
        else if (AnimDirection == 0)
        {
            SpawnBGShop(setData.ShopPrefab, Vector3.zero);
        }
    }

    private void DeactiveOldShop()
    {
        // Careful to call Despawn twice here - it will cause the GameObject to be destroyed
        if (OldBGShop != null
            && OldBGShop.gameObject.activeInHierarchy)
        {
            LeanPool.Despawn(OldBGShop.gameObject);
            OldBGShop = null;
        }
    }

    private void SpawnBGShop(GameObject shopPrefab, Vector3 offset, int offsetIndex = 0)
    {
        if (shopPrefab != null)
        {
            CurrentBGShop = LeanPool.Spawn(shopPrefab, offset, Quaternion.identity, BGShopParent).transform;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (BGShopParent == null)
        {
            BGShopParent = transform;
        }
    }
#endif
}