using TMPro;
using UnityEngine;
using PrimeTween;
using Utilities;

public class UIScoreText : MonoBehaviour
{
    public TextMeshPro ScoreText;
    

    public void UpdateTotalScore(int Score)
    {
        string formatText = Score.ToString();
        ScoreText.text = "Score: " + formatText;
        float scaleFactor = 1.35f + 0.1f * Mathf.Clamp(Score / 100, 0, 7);
        Tween.Scale(ScoreText.rectTransform, Vector3.one * scaleFactor, 0.15f, cycleMode: CycleMode.Yoyo, cycles: 2, ease: Ease.InOutBack)
            .OnComplete(ResetText);
    }

    public void ResetText()
    {
        //ScoreText.text = "Score: " + Score.ToString();
        ScoreText.rectTransform.localScale = Vector3.one;
    }
}
