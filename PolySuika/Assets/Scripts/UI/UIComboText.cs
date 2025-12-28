using PrimeTween;
using Sortify;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

public class UIComboText : MonoBehaviour
{
    public TextMeshPro ComboText;

    [BetterHeader("Listen To")]
    public Int2EventChannelSO ECOnCurrentScoreAndMultiChange;
    public VoidEventChannelSO ECOnComboEnd;

    //void Start()
    //{
    //    var scoreManager = FindAnyObjectByType<ScoreManager>();
    //    if (scoreManager != null)
    //    {
    //        scoreManager.EOnCurrentScoreAndMultiChange += OnScoreAndMultiChange;
    //        scoreManager.EOnComboEnd += OnComboEnd;
    //    }
    //}

    private void OnEnable()
    {
        ECOnCurrentScoreAndMultiChange.Sub(OnScoreAndMultiChange);
        ECOnComboEnd.Sub(OnComboEnd);
    }

    private void OnDisable()
    {
        ECOnCurrentScoreAndMultiChange.UnSub(OnScoreAndMultiChange);
        ECOnComboEnd.UnSub(OnComboEnd);
    }

    public void OnScoreAndMultiChange(int score, int multi)
    {
        if (score > 0)
        {
            Color scoreColor = Color.Lerp(Color.gray6, new Color(0,0.6f,0.1f), score / 20f);
            ComboText.enabled = true;
            ComboText.text = "<color=#" + scoreColor.ToHexString() + "> +" + score.ToString() + "</color>";
            if (multi > 1)
            {
                string formatText = " x" + multi.ToString();
                if (multi <= GConst.TIER_RANK_1)
                {   // White color for low tiers
                    formatText = "<color=#B2B2B2>" + formatText + "</color>"; // White color for mid tiers
                }
                else if (multi <= GConst.TIER_RANK_2)
                {
                    formatText = "<color=#CD7F32>" + formatText + "</color>"; // Gold color for mid tiers
                }
                else if (multi <= GConst.TIER_RANK_3)
                {
                    formatText = "<b><color=#F79A19>" + formatText + "</color></b>"; // Orange color for higher tiers
                }
                else
                {
                    formatText = "<b><color=#B90000>" + formatText + "</color></b>"; // Red color for high tiers
                }
                // At certain combo change to rainbow color
                ComboText.text += formatText;
            }
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
