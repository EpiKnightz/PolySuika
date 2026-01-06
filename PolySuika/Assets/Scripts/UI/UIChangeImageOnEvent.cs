using PrimeTween;
using Sortify;
using UnityEngine;
using Image = UnityEngine.UI.Image;

[RequireComponent(typeof(Image))]
public class UIChangeImageOnEvent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image TargetImage;
    [SerializeField] private Image OldImage;

    [Header("Variables")]
    [SerializeField] private float AnimY = 40f;
    [SerializeField] private float AnimDuration = 0.5f;

    [BetterHeader("Listen To")]
    public GameModeEventChannelSO ECOnCurrentModeChange;
    public IntEventChannelSO ECOnGameModeIndexOffset;

    // Privates
    private float CurrentAnimY;

    private void OnEnable()
    {
        ECOnCurrentModeChange.Sub(OnModeChange);
        ECOnGameModeIndexOffset.Sub(OnChangeModeOffset);
    }

    private void OnDisable()
    {
        ECOnCurrentModeChange.Unsub(OnModeChange);
        ECOnGameModeIndexOffset.Unsub(OnChangeModeOffset);
    }

    private void OnChangeModeOffset(int offset)
    {
        CurrentAnimY = AnimY * offset;
    }

    private void OnModeChange(GameModeSO newMode)
    {
        OldImage.sprite = TargetImage.sprite;
        TargetImage.sprite = newMode.GetIcon();
        OldImage.gameObject.SetActive(true);
        OldImage.transform.localPosition = new Vector3(TargetImage.transform.localPosition.x,
                                                       0,
                                                       TargetImage.transform.localPosition.z);
        TargetImage.transform.localPosition = new Vector3(TargetImage.transform.localPosition.x,
                                                            -CurrentAnimY,
                                                            TargetImage.transform.localPosition.z);
        Tween.LocalPositionY(OldImage.transform, CurrentAnimY, AnimDuration, Ease.OutBack);
        Tween.LocalPositionY(TargetImage.transform, 0, AnimDuration, Ease.OutBack);
        Tween.Delay(AnimDuration, DeactiveOldImage);
    }

    private void DeactiveOldImage()
    {
        OldImage.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (TargetImage == null)
            TargetImage = GetComponent<Image>();
        if (OldImage == null)
            Debug.LogWarning($"{nameof(OldImage)} is not assigned in {nameof(UIChangeImageOnEvent)} on {gameObject.name}.");
    }
#endif
}
