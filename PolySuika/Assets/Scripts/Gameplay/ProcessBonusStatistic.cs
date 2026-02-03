using Sortify;
using UnityEngine;
using Utilities;

public class ProcessBonusStatistic : ProcessStatistic<int>
{
    [Header("Variables")]
    [SerializeField] protected int BonusScore = 1000;
    [SerializeField] protected float ScoreMultiplier = 2;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECOnGameOver;
    public IntEventChannelSO ECOnComboScoreTotalChanged;
    public IntEventChannelSO ECOnPreCalculatedScore;

    // Privates
    protected int LeftoverScore = 0;
    protected int TotalScore = 0;
    protected int PrecalculatedScore = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        ECOnComboScoreTotalChanged.Sub(UpdateComboScore);
        ECOnPreCalculatedScore.Sub(UpdatePreCalculatedScore);
        ECOnGameOver.Sub(InitialBonusText);
        ResetScore();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ECOnComboScoreTotalChanged.Unsub(UpdateComboScore);
        ECOnPreCalculatedScore.Unsub(UpdatePreCalculatedScore);
        ECOnGameOver.Unsub(InitialBonusText);
        ClearStatString();
    }

    private void InitialBonusText()
    {
        UpdateStatString(BonusScore);
    }

    private void UpdateComboScore(int score)
    {
        TotalScore = score;
        PrecalculatedScore = 0;
    }

    private void UpdatePreCalculatedScore(int score)
    {
        PrecalculatedScore = score;
    }

    protected override void OnStatisticUpdated(int statisticValue)
    {
        if (statisticValue != GConst.CLEAR_FINISHED_VALUE)
        {
            // Intended to += to make each clear more hurtful
            LeftoverScore = ProcessStatisticValue(statisticValue);
            if (LeftoverScore != 0)
            {
                UpdateStatString(BonusScore * LeftoverScore);
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
                TotalScore += BonusScore * LeftoverScore;
            }
            ECOnFinalScore.Invoke(TotalScore);
            ResetScore();
        }
    }

    protected override int ProcessStatisticValue(int value)
    {
        return (int)((LeftoverScore * ScoreMultiplier) + Mathf.Pow(ScoreMultiplier, value));

    }

    protected override string FormatedString(int value)
    {
        return value.ToString("#,#");
    }

    protected void ResetScore()
    {
        TotalScore = 0;
        LeftoverScore = 0;
        PrecalculatedScore = 0;
    }
}
