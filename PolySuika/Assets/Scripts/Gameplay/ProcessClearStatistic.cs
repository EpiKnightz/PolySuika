using UnityEngine;
using Utilities;

public class ProcessClearStatistic : ProcessBonusStatistic
{
    [Header("Variables")]
    [SerializeField] private int UltimateScore = 10000000;

    protected override void OnStatisticUpdated(int statisticValue)
    {
        if (statisticValue != GConst.CLEAR_FINISHED_VALUE)
        {
            // Intended to += to make each clear more hurtful
            LeftoverScore += ProcessStatisticValue(statisticValue);
            if (LeftoverScore != 0)
            {
                UpdateStatString(BonusScore / LeftoverScore);
            }
            else
            {
                Debug.Break();
            }
        }
        else
        {
            TotalScore += PrecalculatedScore;
            if (LeftoverScore != 0)
            {
                TotalScore += BonusScore / LeftoverScore;
            }
            else
            {
                TotalScore = (int)((TotalScore * ScoreMultiplier) + UltimateScore);
            }
            ECOnFinalScore.Invoke(TotalScore);
            ResetScore();
        }
    }

    protected override int ProcessStatisticValue(int value)
    {
        return (int)Mathf.Pow(ScoreMultiplier, value);

    }
}
