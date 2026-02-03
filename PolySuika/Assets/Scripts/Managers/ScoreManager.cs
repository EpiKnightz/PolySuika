using Sortify;
using UnityEngine;
using Utilities;

public class ScoreManager : MonoBehaviour
{
    [BetterHeader("Broadcast On")]
    public IntEventChannelSO ECOnCurrentScoreChange = null;
    public IntEventChannelSO ECOnScoreMultiChange = null;
    public Int2EventChannelSO ECOnCurrentScoreAndMultiChange = null;
    public IntEventChannelSO ECOnCurrentScorePreTotal = null;
    public FloatEventChannelSO ECOnComboTimeChange = null;
    public VoidEventChannelSO ECOnComboEnd = null;
    public IntEventChannelSO ECOnScoreTotalChange = null;
    public IntEventChannelSO ECOnFinalScore = null;

    [BetterHeader("Listen To")]
    public IntEventChannelSO ECOnMergeEvent;
    public VoidEventChannelSO ECOnRestartTriggered;
    public VoidEventChannelSO ECOnLoseTrigger;

    private int TotalScore = 0;
    private int CurrentScore = 0;
    private int ScoreMultiplier = 0;
    public float ComboDuration = 3f;
    private int Combo = 0;
    private float CurrentComboTime = 0f;
    private bool bIsFinalCombo = false;

    private const int COMBO_RESET_VALUE = -10;

    private void OnEnable()
    {
        ECOnMergeEvent.Sub(OnMergeEvent);
        ECOnRestartTriggered.Sub(ResetScore);
        ECOnLoseTrigger.Sub(OnEndGame);
    }

    private void OnDisable()
    {
        ECOnMergeEvent.Unsub(OnMergeEvent);
        ECOnRestartTriggered.Unsub(ResetScore);
        ECOnLoseTrigger.Unsub(OnEndGame);
    }

    public void OnMergeEvent(int Tier)
    {
        CurrentScore += (int)Mathf.Pow(Tier < GConst.TIER_RANK_1 ? 2 :
                                        Tier < GConst.TIER_RANK_2 ? 3 :
                                        Tier, Tier);
        ECOnCurrentScoreChange.Invoke(CurrentScore);
        RefreshCombo();
    }

    private void RefreshCombo()
    {
        if (CurrentComboTime > 0)
        {
            int extraCombo = Mathf.CeilToInt(Combo / 10f);
            Combo += 1 + (CurrentComboTime > 2 ? extraCombo : 0);
        }
        else
        {
            Combo = 0;
        }
        ScoreMultiplier = 1 + Combo;
        ECOnScoreMultiChange.Invoke(ScoreMultiplier);
        ECOnCurrentScoreAndMultiChange.Invoke(CurrentScore, ScoreMultiplier);
        int PreCalculated = CurrentScore * ScoreMultiplier;
        ECOnCurrentScorePreTotal.Invoke(PreCalculated);
        CurrentComboTime = ComboDuration;
    }

    private void Update()
    {
        if (CurrentComboTime > 0)
        {
            CurrentComboTime -= Time.deltaTime;
            ECOnComboTimeChange.Invoke(CurrentComboTime / ComboDuration);
        }
        else if (CurrentComboTime != COMBO_RESET_VALUE)
        {
            CurrentComboTime = COMBO_RESET_VALUE;
            ECOnComboTimeChange.Invoke(0f);
            OnComboEnd();
        }
    }

    private void OnComboEnd()
    {
        TotalScore += CurrentScore * ScoreMultiplier;
        CurrentScore = 0;
        ScoreMultiplier = 0;
        Combo = 0;
        ECOnComboEnd.Invoke();
        ECOnScoreTotalChange.Invoke(TotalScore);
        if (bIsFinalCombo)
        {
            ECOnFinalScore.Invoke(TotalScore);
        }
    }

    public void OnEndGame()
    {
        if (CurrentComboTime > 0)
        {
            bIsFinalCombo = true;
        }
        else
        {
            ECOnFinalScore.Invoke(TotalScore);
        }
    }

    public void ResetScore()
    {
        TotalScore = 0;
        CurrentScore = 0;
        ScoreMultiplier = 0;
        Combo = 0;
        CurrentComboTime = 0f;
        ECOnScoreTotalChange.Invoke(TotalScore);
        ECOnCurrentScoreChange.Invoke(CurrentScore);
        ECOnScoreMultiChange.Invoke(ScoreMultiplier);
        ECOnComboTimeChange.Invoke(0f);
        bIsFinalCombo = false;
    }
}
