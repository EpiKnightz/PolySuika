using PrimeTween;
using Sortify;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

public class UIComboText : MonoBehaviour
{
    [Header("References")]
    public TextMeshPro ComboText;

    [Header("Variables")]
    [SerializeField] private Color ScoreMinColor;
    [SerializeField] private Color ScoreMaxColor;
    [SerializeField] private Color MultiMinColor;
    [SerializeField] private Color MultiMaxColor;
    [SerializeField] private float ScoreColorChangeRate = 0.05f;
    [SerializeField] private float MultiColorChangeRate = 0.05f;

    [BetterHeader("Listen To")]
    public Int2EventChannelSO ECOnCurrentScoreAndMultiChange;
    public VoidEventChannelSO ECOnComboEnd;

    private void OnEnable()
    {
        ECOnCurrentScoreAndMultiChange.Sub(OnScoreAndMultiChange);
        ECOnComboEnd.Sub(OnComboEnd);
    }

    private void OnDisable()
    {
        ECOnCurrentScoreAndMultiChange.Unsub(OnScoreAndMultiChange);
        ECOnComboEnd.Unsub(OnComboEnd);
    }

    public void OnScoreAndMultiChange(int score, int multi)
    {
        if (score > 0)
        {
            ComboText.enabled = true;
            Color scoreColor = Color.Lerp(ScoreMinColor, ScoreMaxColor, score * ScoreColorChangeRate);
            Color multiColor = Color.Lerp(MultiMinColor, MultiMaxColor, multi * MultiColorChangeRate);
            ComboText.text = "<color=#" + scoreColor.ToHexString() + "> +" + score.ToString() + "</color>"
                                + "<color=#" + multiColor.ToHexString() + "> x" + multi.ToString() + "</color>";

            float scaleFactor = 1.35f + Mathf.Clamp01(multi * 0.1f);
            Tween.Scale(ComboText.rectTransform, Vector3.one * scaleFactor, 0.15f, cycleMode: CycleMode.Yoyo, cycles: 2, ease: Ease.InOutBack)
                        .OnComplete(ResetText);
        }
        else
        {
            ComboText.enabled = false;
        }
    }

    public void OnComboEnd()
    {
        ComboText.enabled = false;
    }

    public void ResetText()
    {
        ComboText.rectTransform.localScale = Vector3.one;
    }
}
