using PrimeTween;
using Sortify;
using TMPro;
using UnityEngine;
using Utilities;

public class UIScoreText : MonoBehaviour
{
    [Header("References")]
    public TextMeshPro ScoreText;

    [Header("Variables")]
    [SerializeField] private string BreakGameText = "You broke the game!";

    [BetterHeader("Listen To")]
    public IntEventChannelSO ECOnScoreTotalChange;
    public IntEventChannelSO ECOnCurrentScorePreTotal;

    private int OldScore = 0;
    private int NewScore = 0;
    private int NextMilestone = 0;

    private void OnEnable()
    {
        ECOnScoreTotalChange.Sub(UpdateTotalScore);
        ECOnCurrentScorePreTotal.Sub(UpdateFlavorText);
    }

    private void OnDisable()
    {
        ECOnScoreTotalChange.Unsub(UpdateTotalScore);
        ECOnCurrentScorePreTotal.Unsub(UpdateFlavorText);
    }

    public void UpdateTotalScore(int score)
    {
        if (score == 0)
        {
            ScoreText.text = string.Empty;
            return;
        }
        else if (score < 0)
        {
            Tween.StopAll(ScoreText.rectTransform);
            ScoreText.text = BreakGameText;
            return;
        }

        OldScore = NewScore;
        NewScore = score;
        NextMilestone = GConst.NICE_SCORE;
        string formatText = score.ToString();
        int ScorePower = Mathf.Clamp(score / 50, 0, 7);
        float scaleFactor = 1.35f + (0.075f * ScorePower);
        float duration = 0.1f + (ScorePower * 0.025f);
        Tween.StopAll(ScoreText.rectTransform);
        ResetSize();
        Tween.Scale(ScoreText.rectTransform, Vector3.one * scaleFactor, duration, cycleMode: CycleMode.Yoyo, cycles: 2, ease: Ease.InOutBack)
            .OnComplete(ResetSize);
        Tween.Custom(OldScore, NewScore, duration, val =>
        {
            string interimText = Mathf.FloorToInt(val).ToString("#,#");
            ScoreText.text = "Score: " + interimText;
        });
        // There is potential for tween end early and score not final here
    }

    public void UpdateFlavorText(int score)
    {
        if (score >= NextMilestone)
        {
            ScoreText.text = ScoreToText(score);
            Tween.StopAll(ScoreText.rectTransform);
            ResetSize();
            Tween.Scale(ScoreText.rectTransform, Vector3.one * 1.4f, 0.25f, cycleMode: CycleMode.Yoyo,
                cycles: NextMilestone > GConst.ULTIMATE_SCORE ? -1 : 2, ease: Ease.InOutBack)
                .OnComplete(ResetSize);
        }
    }

    private string ScoreToText(int score)
    {
        if (score < GConst.GOOD_SCORE)
        {
            NextMilestone = GConst.GOOD_SCORE;
            //TO-DO Move the hard code color to sth better
            return TextUtilities.ScoreMilestoneToColoredText(ScoreMilestone.NICE);
        }
        else if (score < GConst.GREAT_SCORE)
        {
            NextMilestone = GConst.GREAT_SCORE;
            return TextUtilities.ScoreMilestoneToColoredText(ScoreMilestone.GOOD);
        }
        else if (score < GConst.SUPER_SCORE)
        {
            NextMilestone = GConst.SUPER_SCORE;
            return TextUtilities.ScoreMilestoneToColoredText(ScoreMilestone.GREAT);
        }
        else if (score < GConst.UNREAL_SCORE)
        {
            NextMilestone = GConst.UNREAL_SCORE;
            return TextUtilities.ScoreMilestoneToColoredText(ScoreMilestone.SUPER);
        }
        else if (score < GConst.INSANE_SCORE)
        {
            NextMilestone = GConst.INSANE_SCORE;
            return TextUtilities.ScoreMilestoneToColoredText(ScoreMilestone.UNREAL);
        }
        else if (score < GConst.MAGICAL_SCORE)
        {
            NextMilestone = GConst.MAGICAL_SCORE;
            return TextUtilities.ScoreMilestoneToColoredText(ScoreMilestone.INSANE);
        }
        else if (score < GConst.EXTREME_SCORE)
        {
            NextMilestone = GConst.EXTREME_SCORE;
            return TextUtilities.ScoreMilestoneToColoredText(ScoreMilestone.MAGICAL);
        }
        else if (score < GConst.ULTIMATE_SCORE)
        {
            NextMilestone = GConst.ULTIMATE_SCORE;
            return TextUtilities.ScoreMilestoneToColoredText(ScoreMilestone.EXTREME);
        }
        else
        {
            // TO-DO change this to uint or even higher
            NextMilestone = int.MaxValue;
            //return "<color=#0029A1>U</color><color=#007A98>L</color><color=#069800>T</color><color=#919800>I</color><color=#AD4500>M</color><color=#982100>A</color><color=#930098>T</color><color=#2700FF>E</color><color=#AAAAAA>!</color>";
            return TextUtilities.RainbowString(ScoreMilestone.ULTIMATE.ToString() + "!");
        }
    }

    public void ResetSize()
    {
        //ScoreText.text = "Score: " + Score.ToString();
        ScoreText.rectTransform.localScale = Vector3.one;
    }
}
