using PrimeTween;
using Sortify;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UIChangeTextOnEvent<T> : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected TMP_Text TextComponent;

    [Header("Variables")]
    [SerializeField] private TweenSettings<Vector3> TweenSettings;

    [BetterHeader("Listen To")]
    public EventChannelSO<T> ECOnTriggerChange;
    public VoidEventChannelSO[] ECOnClearEventList;

    private void Awake()
    {
        if (TextComponent.text == string.Empty)
        {
            TextComponent.enabled = false;
        }
        ECOnTriggerChange.Sub(OnTriggerChange);
        for (int i = 0; i < ECOnClearEventList.Length; i++)
        {
            ECOnClearEventList[i].Sub(ClearText);
        }
    }

    private void OnDestroy()
    {
        ECOnTriggerChange.Unsub(OnTriggerChange);
        for (int i = 0; i < ECOnClearEventList.Length; i++)
        {
            ECOnClearEventList[i].Unsub(ClearText);
        }
    }

    protected virtual void OnTriggerChange(T newValue)
    {
    }

    protected void ClearText()
    {
        TextComponent.SetText(string.Empty);
        TextComponent.enabled = false;
    }

    protected void PlayAnim()
    {
        if (TweenSettings.settings.duration != 0.01f)
        {
            Tween.Scale(TextComponent.rectTransform, TweenSettings);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (TextComponent == null)
        {
            TextComponent = GetComponent<TMP_Text>();
        }
    }
#endif
}
