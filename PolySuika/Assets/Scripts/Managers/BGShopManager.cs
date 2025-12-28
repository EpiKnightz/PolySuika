using UnityEngine;
using Utilities;
using Lean.Pool;
using PrimeTween;
using Sortify;

public class BGShopManager : MonoBehaviour
{
    public Vector3 MainPosition = Vector3.zero;
    public Vector3 RightSpawnPosition = new Vector3(50, 0, 0);
    public float AnimDuration = 1f;

    [BetterHeader("Listen To")]
    public IntEventChannelSO ECOnCurrentLevelSetChangedOffset;

    // Private
    private Transform CurrentBGShop;
    private Transform OldBGShop;
    private GetObjectEvent DGetShopPrefab;

    void Start()
    {
        var dataMan = FindAnyObjectByType<DataManager>();
        if (dataMan != null)
        {
            DGetShopPrefab += dataMan.GetCurrentShopBG;
        }
        SpawnDefaultBGShop();
    }

    private void OnEnable()
    {
        ECOnCurrentLevelSetChangedOffset.Sub(ChangeBackgroundShop);
    }

    private void OnDisable()
    {
        ECOnCurrentLevelSetChangedOffset.Unsub(ChangeBackgroundShop);
    }

    void ChangeBackgroundShop(int offset)
    {
        var shopPrefab = DGetShopPrefab?.Invoke();
        if (shopPrefab != null
            && CurrentBGShop != null)
        {
            OldBGShop = CurrentBGShop;
            CurrentBGShop = LeanPool.Spawn(shopPrefab, RightSpawnPosition * offset, Quaternion.identity).transform;
            OldBGShop.gameObject.SetActive(true);
            CurrentBGShop.gameObject.SetActive(true);
            Tween.PositionX(OldBGShop, RightSpawnPosition.x * (-offset), AnimDuration, Ease.OutBack);
            Tween.PositionX(CurrentBGShop, 0, AnimDuration, Ease.OutBack);
            Tween.Delay(AnimDuration, DeactiveOldShop);
        }
    }

    void DeactiveOldShop()
    {
        // Careful to call Despawn twice here - it will cause the GameObject to be destroyed
        if (OldBGShop.gameObject.activeInHierarchy)
        {
            LeanPool.Despawn(OldBGShop.gameObject);
        }
    }

    void SpawnDefaultBGShop()
    {
        var shopPrefab = DGetShopPrefab?.Invoke();
        if (shopPrefab != null)
        {
            CurrentBGShop = LeanPool.Spawn(shopPrefab, Vector3.zero, Quaternion.identity).transform;
        }
    }
}
