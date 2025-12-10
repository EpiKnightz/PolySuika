using TMPro;
using UnityEngine;
using PrimeTween;
using Utilities;

public class UIScoreText : MonoBehaviour
{
    public TextMeshPro ScoreText;
    private int Score = 0;

    public void UpdateScoreWhenMerge(int Tier)
    {
        Score += (int)Mathf.Pow(2, Tier + 1);
        string formatText = Score.ToString();
        if (Tier < GConst.TIER_RANK_1)
        {
            formatText = "<b><color=#3402d9>" + formatText + "</color></b>"; // Blue color for low tiers
        }
        else if (Tier < GConst.TIER_RANK_2)
        {
            formatText = "<b><color=#CD7F32>" + formatText + "</color></b>"; // Gold color for mid tiers
        } else
        {
            formatText = "<b><color=#C00000>" + formatText + "</color></b>"; // Red color for high tiers
        }
        // At certain combo change to rainbow color
        ScoreText.text = "Score: " + formatText;
        float scaleFactor = 1.35f + 0.1f * Tier;
        Tween.Scale(ScoreText.rectTransform, Vector3.one * scaleFactor, 0.1f, cycleMode: CycleMode.Yoyo, cycles: 2, ease: Ease.InOutBack)
            .OnComplete(ResetText);
    }

    public void ResetText()
    {
        ScoreText.text = "Score: " + Score.ToString();
        ScoreText.rectTransform.localScale = Vector3.one;
    }
}
